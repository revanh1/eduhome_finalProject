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

    public class NoticeController : Controller
    {
        private readonly EduHomeDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public NoticeController(EduHomeDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            int TotalCount = _context.Notices.Where(x => !x.IsDeleted).Count();
            ViewBag.TotalPage = (int)Math.Ceiling((decimal)TotalCount / 8);
            ViewBag.CurrentPage = page;
            IEnumerable<Notice> notices = await _context.Notices.Where(x => !x.IsDeleted)
                .Skip((page - 1) * 8).Take(8)
                 .ToListAsync();
            return View(notices);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Notice notice)
        {
            if (!ModelState.IsValid)
            {
                return View(notice);
            }
            notice.CreatedDate = DateTime.Now;
            await _context.AddAsync(notice);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            Notice? notice = await _context.Notices.Where(x => x.Id == id && !x.IsDeleted)
             .FirstOrDefaultAsync();
            if (notice is null)
            {
                return NotFound();
            }
            return View(notice);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id,Notice notice)
        {
            Notice? updatedNotice = await _context.Notices.Where(x => x.Id == id && !x.IsDeleted)
                  .FirstOrDefaultAsync();
            if(notice is null)
            {
                return View(notice);
            }
            if (!ModelState.IsValid)
            {
                return View(updatedNotice);
            }

 
            updatedNotice.Text = notice.Text;
            updatedNotice.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Remove(int id)
        {
            Notice? notice = await _context.Notices.Where(x=>x.Id ==id && !x.IsDeleted)
                .FirstOrDefaultAsync();
            if(notice is null)
            {
                return NotFound();
            }
            notice.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
