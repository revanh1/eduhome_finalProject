using EduHome.App.Context;
using EduHome.App.ViewModels;
using EduHome.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduHome.App.Controllers
{
	public class TeacherController : Controller
	{
		private readonly EduHomeDbContext _context;

		public TeacherController(EduHomeDbContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index(int page = 1)
		{
            int TotalCount = _context.Teachers.Where(x => !x.IsDeleted).Count();
            ViewBag.TotalPage = (int)Math.Ceiling((decimal)TotalCount / 12);
            ViewBag.CurrentPage = page;
            IEnumerable<Teacher> teachers = await _context.Teachers.Where(x => !x.IsDeleted)
				.Include(x=>x.Socials.Take(4))
				.Include(x=>x.Position)
                 .Skip((page - 1) * 12).Take(12)
                .ToListAsync();
			return View(teachers);
		}
		public async Task<IActionResult> Detail(int id)
		{
			Teacher? teacher = await _context.Teachers.Where(x => x.Id == id && !x.IsDeleted)
			.Include(x=>x.teacherHobbies)
			 .ThenInclude(x=>x.Hobby)
			 .Include(x=>x.Position)
			 .Include(x=>x.Degree)
			 .Include(x=>x.Socials)
			 .Include(x=>x.Skills)
	  .FirstOrDefaultAsync();
			if (teacher is null)
			{
				return NotFound();
			}
			TeacherVM teacherVM = new TeacherVM
			{
				Teacher = teacher
			};

			return View(teacherVM);
		}
	}
}
