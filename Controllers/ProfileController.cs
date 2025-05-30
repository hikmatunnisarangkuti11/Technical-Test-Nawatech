using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project1.Models;
using Project1.ViewModels;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;
using System;

[Authorize]
public class ProfileController : Controller
{
    private readonly UserManager<Users> _userManager;

    public ProfileController(UserManager<Users> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return NotFound();

        ViewBag.ProfilePicture = string.IsNullOrEmpty(user.Picture)
            ? "/images/default-profile.png"
            : user.Picture;

        var model = new ProfileEditViewModel
        {
            Name = user.Name,
            Email = user.Email,
            Picture = user.Picture
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Index(ProfileEditViewModel model, IFormFile? profilePicture)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return NotFound();

        user.Name = model.Name;

        if (profilePicture != null && profilePicture.Length > 0)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{user.Id}_{Guid.NewGuid()}{Path.GetExtension(profilePicture.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await profilePicture.CopyToAsync(stream);
            }

            user.Picture = "/uploads/" + fileName;
        }

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction("Index");
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return View(model);
    }
}
