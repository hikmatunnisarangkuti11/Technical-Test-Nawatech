using Microsoft.AspNetCore.Mvc;
using Project1.Services;
using System.Threading.Tasks;

namespace Project1.Controllers
{
    public class EmailTestController : Controller
    {
        private readonly EmailSender _emailSender;

        public EmailTestController(EmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task<IActionResult> Index()
        {
            await _emailSender.SendEmailAsync("test@example.com", "Test Subject", "Test Body");
            return Content("Email sent!");
        }
    }
}
