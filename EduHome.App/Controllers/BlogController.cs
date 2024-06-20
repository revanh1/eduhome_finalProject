using EduHome.App.Context;
using EduHome.App.ViewModels;
using EduHome.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduHome.App.Controllers
{
	public class BlogController : Controller
	{
		private readonly EduHomeDbContext _context;

		public BlogController(EduHomeDbContext context)
		{
			_context = context;
		}
		public async Task<IActionResult> Index(int? id, int page = 1)
		{
            int TotalCount = _context.Blogs.Where(x => !x.IsDeleted).Count();
            ViewBag.TotalPage = (int)Math.Ceiling((decimal)TotalCount / 4);
            ViewBag.CurrentPage = page;
            ViewBag.Categories = await _context.Categories.Where(x => !x.IsDeleted)
              .Include(x => x.blogCategories)
               .ThenInclude(x => x.Blog)
                 .ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(x => !x.IsDeleted)
    .Include(x => x.courseTags)
     .ThenInclude(x => x.Course)
    .ToListAsync();
            if (id == null)
			{
				IEnumerable<Blog> blogs = await _context.Blogs.Where(x => !x.IsDeleted)
					  .Include(x => x.BlogCategories)
					 .ThenInclude(x => x.Category)
						   .Include(x => x.BlogTags)
					 .ThenInclude(x => x.Tag)
                      .Skip((page - 1) * 4).Take(4)
                    .ToListAsync();
				return View(blogs);
			}
			else
			{
                IEnumerable<Blog> blogs = await _context.Blogs.Where(x => !x.IsDeleted && x.BlogCategories.Any(x => x.Category.Id == id))
                    .Include(x => x.BlogCategories)
                       .ThenInclude(x => x.Category)
                    .Include(x => x.BlogTags)
                       .ThenInclude(x => x.Tag)
                    .ToListAsync();
                return View(blogs);
            }
		}
		public async Task<IActionResult> Detail(int id)
		{
            ViewBag.Categories = await _context.Categories.Where(x => !x.IsDeleted)
                .Include(x => x.blogCategories)
                 .ThenInclude(x => x.Blog)
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
            Blog? blog = await _context.Blogs.Where(x => !x.IsDeleted)
			  .Include(x => x.BlogCategories)
				 .ThenInclude(x => x.Category)
					   .Include(x => x.BlogTags)
				 .ThenInclude(x => x.Tag)
					.FirstOrDefaultAsync();
  
            if (blog is null)
			{
				return NotFound();
			}
			BlogVM blogVM = new BlogVM
			{
				Blog = blog
			};

			return View(blogVM);
		}

	}
}
