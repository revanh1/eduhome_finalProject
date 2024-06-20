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

    public class PositionController : Controller
    {
        private readonly EduHomeDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public PositionController(EduHomeDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            int TotalCount = _context.Positions.Where(x => !x.IsDeleted).Count();
            ViewBag.TotalPage = (int)Math.Ceiling((decimal)TotalCount / 6);
            ViewBag.CurrentPage = page;
            IEnumerable<Position> Positions = await _context.Positions.Where(x => !x.IsDeleted)
                 .Skip((page - 1) * 6).Take(6)
                 .ToListAsync();
            return View(Positions);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Position Position)
        {
            if (!ModelState.IsValid)
            {
                return View(Position);
            }
            Position.CreatedDate = DateTime.Now;
            await _context.AddAsync(Position);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            Position? Position = await _context.Positions.Where(x => x.Id == id && !x.IsDeleted)
             .FirstOrDefaultAsync();
            if (Position is null)
            {
                return NotFound();
            }
            return View(Position);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id,Position Position)
        {
            Position? updatedPosition = await _context.Positions.Where(x => x.Id == id && !x.IsDeleted)
                  .FirstOrDefaultAsync();
            if(Position is null)
            {
                return View(Position);
            }
            if (!ModelState.IsValid)
            {
                return View(updatedPosition);
            }

 
            updatedPosition.Name = Position.Name;
            updatedPosition.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Remove(int id)
        {
            Position? Position = await _context.Positions.Where(x=>x.Id ==id && !x.IsDeleted)
                .FirstOrDefaultAsync();
            if(Position is null)
            {
                return NotFound();
            }
            Position.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
