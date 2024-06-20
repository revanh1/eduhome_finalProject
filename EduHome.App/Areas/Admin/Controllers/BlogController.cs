using EduHome.App.Context;
using EduHome.App.Extensions;
using EduHome.Core.Entities;
using EduHome.App.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EduHome.App.ViewModels;

namespace EduHome.App.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class BlogController : Controller
    {
        private readonly EduHomeDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public BlogController(IWebHostEnvironment environment, EduHomeDbContext context)
        {
            _environment = environment;
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1 )
        {
            int TotalCount = _context.Blogs.Where(x => !x.IsDeleted).Count();
            ViewBag.TotalPage = (int)Math.Ceiling((decimal)TotalCount / 5);
            ViewBag.CurrentPage = page;
            IEnumerable<Blog> blogs = await _context.Blogs.Where(x=>!x.IsDeleted)
                .Skip((page - 1) * 5).Take(5)
                .ToListAsync();
            return View(blogs);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.Where(p => !p.IsDeleted).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(p => !p.IsDeleted).ToListAsync();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Blog blog)
        {
            ViewBag.Categories = await _context.Categories.Where(p => !p.IsDeleted).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(p => !p.IsDeleted).ToListAsync();
            if (!ModelState.IsValid)
            {
                return View(blog);
            }
            if (blog.file is null)
            {
                ModelState.AddModelError("file", "Image must be added");
                return View(blog);
            }
            if (!Helper.isImage(blog.file))
            {
                ModelState.AddModelError("file", "File must be image");
                return View(blog);
            }
            if (!Helper.isSizeOk(blog.file, 1))
            {
                ModelState.AddModelError("file", "Size of Image must less than 1 mb!!!");
                return View(blog);
            }
            foreach (var item in blog.CategoryIds)
            {
                if (!await _context.Categories.AnyAsync(x => x.Id == item))
                {
                    ModelState.AddModelError("", "Invalid Category Id");
                    return View(blog);
                }
                BlogCategory blogCategory = new BlogCategory
                {
                    CreatedDate = DateTime.Now,
                    Blog = blog,
                    CategoryId = item
                };
                await _context.BlogCategories.AddAsync(blogCategory);
            }
            foreach (var item in blog.TagIds)
            {
                if (!await _context.Tags.AnyAsync(x => x.Id == item))
                {
                    ModelState.AddModelError("", "Invalid Tag Id");
                    return View(blog);
                }
                BlogTag blogTag = new BlogTag
                {
                    CreatedDate = DateTime.Now,
                    Blog = blog,
                    TagId = item
                };
                await _context.BlogTags.AddAsync(blogTag);
            }

            blog.Image = blog.file.CreateImage(_environment.WebRootPath, "img/blog/");
            blog.CreatedDate = DateTime.Now;
            await _context.AddAsync(blog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            ViewBag.Categories = await _context.Categories.Where(p => !p.IsDeleted).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(p => !p.IsDeleted).ToListAsync();
            Blog? blog = await _context.Blogs.Where(x => x.Id == id && !x.IsDeleted)
                .AsNoTracking()
                .Include(x=>x.BlogCategories)
                   .ThenInclude(x=>x.Category)
                 .Include(x=>x.BlogTags)
                    .ThenInclude(x=>x.Tag)
             .FirstOrDefaultAsync();
            if (blog is null)
            {
                return NotFound();
            }
            return View(blog);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, Blog blog)
        {
            ViewBag.Categories = await _context.Categories.Where(p => !p.IsDeleted).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(p => !p.IsDeleted).ToListAsync();
            Blog? updatedBlog = await _context.Blogs.Where(x => x.Id == id && !x.IsDeleted)
                .AsNoTracking()
                .Include(x => x.BlogCategories)
                   .ThenInclude(x => x.Category)
                 .Include(x => x.BlogTags)
                    .ThenInclude(x => x.Tag)
             .FirstOrDefaultAsync();
            if (blog is null)
            {
                return View(blog);
            }
            if (!ModelState.IsValid)
            {
                return View(updatedBlog);
            }

            if (blog.file is not null)
            {
                if (!Helper.isImage(blog.file))
                {
                    ModelState.AddModelError("file", "File must be image");
                    return View(blog);
                }
                if (!Helper.isSizeOk(blog.file, 1))
                {
                    ModelState.AddModelError("file", "Size of Image must less than 1 mb!!!");
                    return View(blog);
                }
                Helper.RemoveImage(_environment.WebRootPath, "img/blog/", updatedBlog.Image);
                blog.Image = blog.file.CreateImage(_environment.WebRootPath, "img/blog/");
            }
            else
            {
                blog.Image = updatedBlog.Image;
			}
            List<BlogCategory> RemoveableCategory = await _context.BlogCategories.
               Where(x => !blog.CategoryIds.Contains(x.CategoryId) && x.BlogId == blog.Id).ToListAsync();

            _context.BlogCategories.RemoveRange(RemoveableCategory);
            foreach (var item in blog.CategoryIds)
            {
                if (_context.BlogCategories.Where(x => x.BlogId == id && x.CategoryId == item).Count() > 0)
                    continue;

                await _context.BlogCategories.AddAsync(new BlogCategory
                {
                    BlogId = id,
                    CategoryId = item
                });
            }
            List<BlogTag> RemoveableTag = await _context.BlogTags.
            Where(x => !blog.TagIds.Contains(x.TagId) && x.BlogId == blog.Id).ToListAsync();

            _context.BlogTags.RemoveRange(RemoveableTag);
            foreach (var item in blog.TagIds)
            {
                if (_context.BlogTags.Where(x => x.BlogId == id && x.TagId == item).Count() > 0)
                    continue;

                await _context.BlogTags.AddAsync(new BlogTag
                {
                    BlogId = id,
                    TagId = item
                });
            }
            blog.CreatedDate = updatedBlog.CreatedDate;
            blog.UpdatedDate = DateTime.Now;
            _context.Blogs.Update(blog);
			await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Remove(int id)
        {
            Blog? blog = await _context.Blogs.Where(x => x.Id == id && !x.IsDeleted)
                .FirstOrDefaultAsync();
            if (blog is null)
            {
                return NotFound();
            }
            blog.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
