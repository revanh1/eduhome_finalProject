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

    public class SkillController : Controller
    {
        private readonly EduHomeDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public SkillController(EduHomeDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            int TotalCount = _context.Skills.Where(x => !x.IsDeleted).Count();
            ViewBag.TotalPage = (int)Math.Ceiling((decimal)TotalCount / 6);
            ViewBag.CurrentPage = page;
            IEnumerable<Skill> Skills = await _context.Skills.Where(x => !x.IsDeleted)
                .Include(x=>x.Teacher)
                 .Skip((page - 1) * 6).Take(6)
                 .ToListAsync();
            return View(Skills);
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
        public async Task<IActionResult> Create(Skill Skill)
        {
			ViewBag.Teachers = await _context.Teachers.Where(x => !x.IsDeleted).
			ToListAsync();
			if (!ModelState.IsValid)
            {
                return View(Skill);
            }
            if (Skill.TeacherId == 0)
            {
                ModelState.AddModelError("","Teacher must be selected");
                return View(Skill);
            }
            Skill.CreatedDate = DateTime.Now;
            await _context.AddAsync(Skill);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            ViewBag.Teachers = await _context.Teachers.Where(x => !x.IsDeleted).
       ToListAsync();
            Skill? Skill = await _context.Skills.Where(x => x.Id == id && !x.IsDeleted)
             .FirstOrDefaultAsync();
            if (Skill is null)
            {
                return NotFound();
            }
            return View(Skill);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id,Skill Skill)
        {
            ViewBag.Teachers = await _context.Teachers.Where(x => !x.IsDeleted).
       ToListAsync();
            Skill? updatedSkill = await _context.Skills.Where(x => x.Id == id && !x.IsDeleted)
                  .FirstOrDefaultAsync();
            if(Skill is null)
            {
                return View(Skill);
            }
			if (Skill.TeacherId == 0)
			{
				ModelState.AddModelError("", "Teacher must be selected");
				return View(Skill);
			}
			if (!ModelState.IsValid)
            {
                return View(updatedSkill);
            }

 
            updatedSkill.Name = Skill.Name;
            updatedSkill.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Remove(int id)
        {
            Skill? Skill = await _context.Skills.Where(x=>x.Id ==id && !x.IsDeleted)
                .FirstOrDefaultAsync();
            if(Skill is null)
            {
                return NotFound();
            }
            Skill.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
