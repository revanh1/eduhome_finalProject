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

    public class DegreeController : Controller
    {
        private readonly EduHomeDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public DegreeController(EduHomeDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> Index(int page = 1 )
        {
            int TotalCount = _context.Degrees.Where(x => !x.IsDeleted).Count();
            ViewBag.TotalPage = (int)Math.Ceiling((decimal)TotalCount /6 );
            ViewBag.CurrentPage = page;
            IEnumerable<Degree> Degrees = await _context.Degrees.Where(x => !x.IsDeleted)
                 .Skip((page - 1) * 6).Take(6)
                 .ToListAsync();
            return View(Degrees);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Degree Degree)
        {
            if (!ModelState.IsValid)
            {
                return View(Degree);
            }
            Degree.CreatedDate = DateTime.Now;
            await _context.AddAsync(Degree);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            Degree? Degree = await _context.Degrees.Where(x => x.Id == id && !x.IsDeleted)
             .FirstOrDefaultAsync();
            if (Degree is null)
            {
                return NotFound();
            }
            return View(Degree);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id,Degree Degree)
        {
            Degree? updatedDegree = await _context.Degrees.Where(x => x.Id == id && !x.IsDeleted)
                  .FirstOrDefaultAsync();
            if(Degree is null)
            {
                return View(Degree);
            }
            if (!ModelState.IsValid)
            {
                return View(updatedDegree);
            }

 
            updatedDegree.Name = Degree.Name;
            updatedDegree.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Remove(int id)
        {
            Degree? Degree = await _context.Degrees.Where(x=>x.Id ==id && !x.IsDeleted)
                .FirstOrDefaultAsync();
            if(Degree is null)
            {
                return NotFound();
            }
            Degree.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
