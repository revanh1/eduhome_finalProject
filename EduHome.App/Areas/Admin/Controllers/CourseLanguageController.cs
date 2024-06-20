using EduHome.App.Context;
using EduHome.App.Extensions;
using EduHome.Core.Entities;
using EduHome.App.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace EduHome.App.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]

    public class CourseLanguageController : Controller
    {
        private readonly EduHomeDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public CourseLanguageController(EduHomeDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<CourseLanguage> CourseLanguages = await _context.CourseLanguages.Where(x => !x.IsDeleted)
                 .ToListAsync();
            return View(CourseLanguages);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseLanguage CourseLanguage)
        {
            if (!ModelState.IsValid)
            {
                return View(CourseLanguage);
            }
            CourseLanguage.CreatedDate = DateTime.Now;
            await _context.AddAsync(CourseLanguage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            CourseLanguage? CourseLanguage = await _context.CourseLanguages.Where(x => x.Id == id && !x.IsDeleted)
             .FirstOrDefaultAsync();
            if (CourseLanguage is null)
            {
                return NotFound();
            }
            return View(CourseLanguage);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id,CourseLanguage CourseLanguage)
        {
            CourseLanguage? updatedCourseLanguage = await _context.CourseLanguages.Where(x => x.Id == id && !x.IsDeleted)
                  .FirstOrDefaultAsync();
            if(CourseLanguage is null)
            {
                return View(CourseLanguage);
            }
            if (!ModelState.IsValid)
            {
                return View(updatedCourseLanguage);
            }

 
            updatedCourseLanguage.Name = CourseLanguage.Name;
            updatedCourseLanguage.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Remove(int id)
        {
            CourseLanguage? CourseLanguage = await _context.CourseLanguages.Where(x=>x.Id ==id && !x.IsDeleted)
                .FirstOrDefaultAsync();
            if(CourseLanguage is null)
            {
                return NotFound();
            }
            CourseLanguage.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
