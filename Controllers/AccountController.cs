using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Project1.Models;        
using Project1.ViewModels;    
using Project1.Services;     

namespace Project1.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Users> _userManager;
        private readonly SignInManager<Users> _signInManager;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<Users> userManager, SignInManager<Users> signInManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new Users
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Name
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account",
                    new { userId = user.Id, token }, Request.Scheme);

                await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                    $"Please confirm your account by clicking this link: <a href='{confirmationLink}'>link</a>");

                TempData["Success"] = "Registration successful! Please check your email to confirm your account.";
                return RedirectToAction("Login");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
                return RedirectToAction("Index", "Home");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound($"User with ID '{userId}' not found.");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                TempData["Success"] = "Email confirmed successfully. You can now log in.";
                return RedirectToAction("Login");
            }

            TempData["Error"] = "Email confirmation failed.";
            return View();
        }


        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Email belum terdaftar.");
                return View(model);
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError("", "Email belum dikonfirmasi. Silakan cek email Anda.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                if (Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                else
                    return RedirectToAction("Index", "Profile");
            }
            else if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "User sedang dikunci (locked out).");
            }
            else if (result.IsNotAllowed)
            {
                ModelState.AddModelError("", "User tidak diizinkan untuk login.");
            }
            else if (result.RequiresTwoFactor)
            {
                ModelState.AddModelError("", "Diperlukan autentikasi dua faktor.");
            }
            else
            {
                ModelState.AddModelError("", "Password salah.");
            }

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

    }
}
