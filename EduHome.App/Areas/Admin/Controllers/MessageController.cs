using EduHome.App.Context;
using EduHome.App.Extensions;
using EduHome.Core.Entities;
using EduHome.App.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduHome.App.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]

    public class MessageController : Controller
    {
        private readonly EduHomeDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public MessageController(EduHomeDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            int TotalCount = _context.ContactMessages.Where(x => !x.IsDeleted).Count();
            ViewBag.TotalPage = (int)Math.Ceiling((decimal)TotalCount / 8);
            ViewBag.CurrentPage = page;
            IEnumerable<ContactMessage> messages = await _context.ContactMessages.Where(x => !x.IsDeleted)
                .Skip((page - 1) * 8).Take(8)
                 .ToListAsync();
            return View(messages);
        }
        public async Task<IActionResult> Info(int id)
        {
            ContactMessage? message = await _context.ContactMessages.Where(x => x.Id == id && !x.IsDeleted)
               .FirstOrDefaultAsync();
            if (message is null)
            {
                return NotFound();
            }
            return View(message);
        }
        public async Task<IActionResult> Remove(int id)
        {
            ContactMessage? message = await _context.ContactMessages.Where(x=>x.Id ==id && !x.IsDeleted)
                .FirstOrDefaultAsync();
            if(message is null)
            {
                return NotFound();
            }
            message.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
