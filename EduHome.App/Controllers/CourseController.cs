using EduHome.App.Context;
using EduHome.App.ViewModels;
using EduHome.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;

namespace EduHome.App.Controllers
{
	public class CourseController : Controller
	{
		private readonly EduHomeDbContext _context;

		public CourseController(EduHomeDbContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index(int? id = null,string? search = null, int page = 1)
		{
            int TotalCount = _context.Courses.Where(x => !x.IsDeleted).Count();
            ViewBag.TotalPage = (int)Math.Ceiling((decimal)TotalCount / 3);
            ViewBag.CurrentPage = page;
            if(search != null)
            {
                TotalCount = _context.Courses.Where(x => !x.IsDeleted && x.Name.Contains(search)).Count();
                ViewBag.TotalPage = (int)Math.Ceiling((decimal)TotalCount / 3);
                List<Course> courses = await _context.Courses.Where(x => !x.IsDeleted && x.Name.Trim().ToLower().Contains(search.Trim().ToLower()))
                    .Include(x => x.courseAssests)
                      .Include(x => x.courseCategories)
                     .ThenInclude(x => x.Category)
                           .Include(x => x.courseTags)
                     .ThenInclude(x => x.Tag)
                     .Skip((page - 1) * 3).Take(3)
                    .ToListAsync();
                if(courses is null)
                {
                    return View(null);
                }
                  return View(courses);
            }
            if (id == null)
			{
				IEnumerable<Course> courses = await _context.Courses.Where(x => !x.IsDeleted)
						.Include(x => x.courseAssests)
						  .Include(x => x.courseCategories)
						 .ThenInclude(x => x.Category)
							   .Include(x => x.courseTags)
						 .ThenInclude(x => x.Tag)
                         .Skip((page - 1) * 3).Take(3)
                        .ToListAsync();
                return View(courses);
            }
			else
			{
                IEnumerable<Course> courses = await _context.Courses.Where(x => !x.IsDeleted && x.courseCategories.Any(x => x.Category.Id == id))
             .Include(x => x.courseAssests)
               .Include(x => x.courseCategories)
              .ThenInclude(x => x.Category)
                    .Include(x => x.courseTags)
              .ThenInclude(x => x.Tag)
             .ToListAsync();
                return View(courses);
            }
        }

        public async Task<IActionResult> Search(string search,int page = 1)
        {
            int TotalCount = _context.Courses.Where(x => !x.IsDeleted && x.Name.Trim().ToLower().Contains(search.Trim().ToLower())).Count();
            ViewBag.TotalPage = (int)Math.Ceiling((decimal)TotalCount / 3);
            ViewBag.CurrentPage = page;
            List<Course> courses = await _context.Courses.Where(x => !x.IsDeleted && x.Name.Trim().ToLower().Contains(search.Trim().ToLower()))
                .Include(x => x.courseAssests)
                  .Include(x => x.courseCategories)
                 .ThenInclude(x => x.Category)
                       .Include(x => x.courseTags)
                 .ThenInclude(x => x.Tag)
                    .Skip((page - 1) * 3).Take(3)
                .ToListAsync();
            return Json(courses);
        }
		public async Task<IActionResult> Detail(int id)
		{
			ViewBag.Categories = await _context.Categories.Where(x => !x.IsDeleted)
				.Include(x=>x.courseCategories)
				 .ThenInclude(x=>x.Course)
				.ToListAsync();
			ViewBag.Blogs = await _context.Blogs.Where(x => !x.IsDeleted)
                  .Include(x => x.BlogCategories)
                 .ThenInclude(x => x.Category)
                       .Include(x => x.BlogTags)
                 .ThenInclude(x => x.Tag)
				 .Take(3)
                .ToListAsync();
			ViewBag.Tags = await _context.Tags.Where(x => !x.IsDeleted)
                .Include(x => x.courseTags)
                 .ThenInclude(x => x.Course)
                .ToListAsync();
            Course? Course = await _context.Courses.Where(x => !x.IsDeleted && x.Id ==id)
                    .Include(x => x.courseAssests)
                      .Include(x => x.courseCategories)
                     .ThenInclude(x => x.Category)
                           .Include(x => x.courseTags)
                     .ThenInclude(x => x.Tag)
					 .Include(x=>x.CourseLanguage)
                    .FirstOrDefaultAsync();
            if (Course is null)
			{
				return NotFound();
			}
			CourseVM courseVM = new CourseVM
			{
                Course = Course
            };
			return View(courseVM);
		}

	}
}
