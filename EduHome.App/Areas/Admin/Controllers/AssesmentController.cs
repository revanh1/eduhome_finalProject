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
    [Authorize(Roles ="Admin,SuperAdmin")]
    public class AssesmentController : Controller
    {
        private readonly EduHomeDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public AssesmentController(EduHomeDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<CourseAssests> CourseAssestss = await _context.CourseAssests.Where(x => !x.IsDeleted)
                 .ToListAsync();
            return View(CourseAssestss);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseAssests CourseAssests)
        {
            if (!ModelState.IsValid)
            {
                return View(CourseAssests);
            }
            CourseAssests.CreatedDate = DateTime.Now;
            await _context.AddAsync(CourseAssests);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            CourseAssests? CourseAssests = await _context.CourseAssests.Where(x => x.Id == id && !x.IsDeleted)
             .FirstOrDefaultAsync();
            if (CourseAssests is null)
            {
                return NotFound();
            }
            return View(CourseAssests);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id,CourseAssests CourseAssests)
        {
            CourseAssests? updatedCourseAssests = await _context.CourseAssests.Where(x => x.Id == id && !x.IsDeleted)
                  .FirstOrDefaultAsync();
            if(CourseAssests is null)
            {
                return View(CourseAssests);
            }
            if (!ModelState.IsValid)
            {
                return View(updatedCourseAssests);
            }

 
            updatedCourseAssests.Name = CourseAssests.Name;
            updatedCourseAssests.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Remove(int id)
        {
            CourseAssests? CourseAssests = await _context.CourseAssests.Where(x=>x.Id ==id && !x.IsDeleted)
                .FirstOrDefaultAsync();
            if(CourseAssests is null)
            {
                return NotFound();
            }
            CourseAssests.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
