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

    public class HobbyController : Controller
    {
        private readonly EduHomeDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public HobbyController(EduHomeDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            int TotalCount = _context.Hobbies.Where(x => !x.IsDeleted).Count();
            ViewBag.TotalPage = (int)Math.Ceiling((decimal)TotalCount / 5);
            ViewBag.CurrentPage = page;
            IEnumerable<Hobby> Hobbys = await _context.Hobbies.Where(x => !x.IsDeleted)
                .Skip((page - 1) * 5).Take(5)
                 .ToListAsync();
            return View(Hobbys);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Hobby Hobby)
        {
            if (!ModelState.IsValid)
            {
                return View(Hobby);
            }
            Hobby.CreatedDate = DateTime.Now;
            await _context.AddAsync(Hobby);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            Hobby? Hobby = await _context.Hobbies.Where(x => x.Id == id && !x.IsDeleted)
             .FirstOrDefaultAsync();
            if (Hobby is null)
            {
                return NotFound();
            }
            return View(Hobby);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id,Hobby Hobby)
        {
            Hobby? updatedHobby = await _context.Hobbies.Where(x => x.Id == id && !x.IsDeleted)
                  .FirstOrDefaultAsync();
            if(Hobby is null)
            {
                return View(Hobby);
            }
            if (!ModelState.IsValid)
            {
                return View(updatedHobby);
            }

 
            updatedHobby.Name = Hobby.Name;
            updatedHobby.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Remove(int id)
        {
            Hobby? Hobby = await _context.Hobbies.Where(x=>x.Id ==id && !x.IsDeleted)
                .FirstOrDefaultAsync();
            if(Hobby is null)
            {
                return NotFound();
            }
            Hobby.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
