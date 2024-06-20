using EduHome.App.Context;
using EduHome.App.Extensions;
using EduHome.Core.Entities;
using EduHome.App.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace EduHome.App.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]

    public class SocialController : Controller
    {
        private readonly EduHomeDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public SocialController(EduHomeDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            int TotalCount = _context.Socials.Where(x => !x.IsDeleted).Count();
            ViewBag.TotalPage = (int)Math.Ceiling((decimal)TotalCount / 8);
            ViewBag.CurrentPage = page;
            IEnumerable<Social> Socials = await _context.Socials.Where(x => !x.IsDeleted)
                .Include(x=>x.Teacher)
                .Skip((page - 1) * 8).Take(8)
                 .ToListAsync();
            return View(Socials);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
			ViewBag.Teachers = await _context.Teachers.Where(x => !x.IsDeleted).
	  ToListAsync();
			return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Social Social)
        {
			ViewBag.Teachers = await _context.Teachers.Where(x => !x.IsDeleted).
	        ToListAsync();
			if (!ModelState.IsValid)
            {
                return View(Social);
            }
            if(Social.TeacherId == 0 && Social.ServiceId == 0)
            {
                ModelState.AddModelError("", "Teacher or Setting Id must be selected");
                return View(Social);
            }
            if (Social.TeacherId == 0)
            {
                //    ModelState.AddModelError("", "Teacher must be selected");
                Social.TeacherId = null;
                Social.ServiceId = _context.Services.Where(x=>!x.IsDeleted).FirstOrDefault()?.Id;
                //    return View(Social);
            }
            Social.CreatedDate = DateTime.Now;
            await _context.AddAsync(Social);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
			ViewBag.Teachers = await _context.Teachers.Where(x => !x.IsDeleted).
	  ToListAsync();
			Social? Social = await _context.Socials.Where(x => x.Id == id && !x.IsDeleted)
             .FirstOrDefaultAsync();
            if (Social is null)
            {
                return NotFound();
            }
            return View(Social);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id,Social Social)
        {
			ViewBag.Teachers = await _context.Teachers.Where(x => !x.IsDeleted).
	  ToListAsync();
			Social? updatedSocial = await _context.Socials.Where(x => x.Id == id && !x.IsDeleted)
                  .FirstOrDefaultAsync();
            if(Social is null)
            {
                return View(Social);
            }
            if (Social.TeacherId == 0 && Social.ServiceId == 0)
            {
                ModelState.AddModelError("", "Teacher or Setting Id must be selected");
                return View(Social);
            }
            if (Social.TeacherId == 0)
            {
                Social.TeacherId = null;
                Social.ServiceId = _context.Services.Where(x => !x.IsDeleted).FirstOrDefault()?.Id;
            }
            if (!ModelState.IsValid)
            {
                return View(updatedSocial);
            }

 
            updatedSocial.Name = Social.Name;
            updatedSocial.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Remove(int id)
        {
            Social? Social = await _context.Socials.Where(x=>x.Id ==id && !x.IsDeleted)
                .FirstOrDefaultAsync();
            if(Social is null)
            {
                return NotFound();
            }
            Social.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
