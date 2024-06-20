using EduHome.App.Context;
using EduHome.App.ViewModels;
using EduHome.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace EduHome.App.Controllers
{
    public class ContactController : Controller
    {
        private readonly EduHomeDbContext _context;

        public ContactController(EduHomeDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ContactVM contactVM = new ContactVM()
            {
                Service = _context.Services.Where(x => !x.IsDeleted)
                  .FirstOrDefault(),
            };
            return View(contactVM);
        }
        [HttpPost]
        public async Task<IActionResult> SendEmail(ContactMessage message)
        {
            string strRegex = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";

            Regex re = new Regex(strRegex);
            if (!ModelState.IsValid)
            {
                TempData["Message"] = "Please fill all inputs qaqa";
                return RedirectToAction(nameof(Index));
            }
            if (!re.IsMatch(message.Email))
            {
                TempData["Email"] = "Please add valid email";
                return RedirectToAction("index", "home");
            }
      
            await _context.ContactMessages.AddAsync(message);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Successfully send message";
            return RedirectToAction(nameof(Index));
        }
    }
}
