using EduHome.App.Context;
using EduHome.App.Extensions;
using EduHome.App.Services.Interfaces;
using EduHome.Core.Entities;
using EduHome.App.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;
using System.Data;
using Microsoft.AspNetCore.Identity;

namespace EduHome.App.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]

    public class CourseController : Controller
    {
        private readonly EduHomeDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IEmailService _emailService;
        private readonly UserManager<AppUser> _userManager;

        public CourseController(IWebHostEnvironment environment, EduHomeDbContext context, IEmailService emailService, UserManager<AppUser> userManager)
        {
            _environment = environment;
            _context = context;
            _emailService = emailService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            int TotalCount = _context.Courses.Where(x => !x.IsDeleted).Count();
            ViewBag.TotalPage = (int)Math.Ceiling((decimal)TotalCount / 5);
            ViewBag.CurrentPage = page;
            IEnumerable<Course> courses = await _context.Courses.Where(x=>!x.IsDeleted)
                .Include(x=>x.CourseLanguage)
                .Skip((page - 1) * 5).Take(5)
                .ToListAsync();
            return View(courses);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.Where(p => !p.IsDeleted).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(p => !p.IsDeleted).ToListAsync();
            ViewBag.Languages = await _context.CourseLanguages.Where(p => !p.IsDeleted).ToListAsync();
            ViewBag.CourseAssests = await _context.CourseAssests.Where(p => !p.IsDeleted).ToListAsync();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Course course)
        {
            ViewBag.Categories = await _context.Categories.Where(p => !p.IsDeleted).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(p => !p.IsDeleted).ToListAsync();
            ViewBag.Languages = await _context.CourseLanguages.Where(p => !p.IsDeleted).ToListAsync();
            ViewBag.CourseAssests = await _context.CourseAssests.Where(p => !p.IsDeleted).ToListAsync();
            if (!ModelState.IsValid)
            {
                return View(course);
            }
            if (course.file is null)
            {
                ModelState.AddModelError("file", "Image must be added");
                return View(course);
            }
            if (!Helper.isImage(course.file))
            {
                ModelState.AddModelError("file", "File must be image");
                return View(course);
            }
            if (!Helper.isSizeOk(course.file, 1))
            {
                ModelState.AddModelError("file", "Size of Image must less than 1 mb!!!");
                return View(course);
            }
            foreach (var item in course.CategoryIds)
            {
                if (!await _context.Categories.AnyAsync(x => x.Id == item))
                {
                    ModelState.AddModelError("", "Invalid Category Id");
                    return View(course);
                }
                CourseCategory courseCategory = new CourseCategory
                {
                    CreatedDate = DateTime.Now,
                    Course = course,
                    CategoryId = item
                };
                await _context.CourseCategories.AddAsync(courseCategory);
            }
            foreach (var item in course.TagIds)
            {
                if (!await _context.Tags.AnyAsync(x => x.Id == item))
                {
                    ModelState.AddModelError("", "Invalid Tag Id");
                    return View(course);
                }
                CourseTag courseTag = new CourseTag
                {
                    CreatedDate = DateTime.Now,
                    Course = course,
                    TagId = item
                };
                await _context.CourseTags.AddAsync(courseTag);
            }

            course.Image = course.file.CreateImage(_environment.WebRootPath, "img/course/");
            course.CreatedDate = DateTime.Now;
            IEnumerable<Subscribe> subscribes = await _context.Subscribes.Where(x => !x.IsDeleted)
             .ToListAsync();
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            await _context.AddAsync(course);
            await _context.SaveChangesAsync();

            foreach (var mail in subscribes)
            {
                string? link = Request.Scheme+"://" + Request.Host + $"/Course/detail/{course.Id}";
                await _emailService.SendMail("nicatsoltanli03@gmail.com", mail.Email,
                    "New Product", "We have new Products", link, "Customer");
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            ViewBag.Categories = await _context.Categories.Where(p => !p.IsDeleted).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(p => !p.IsDeleted).ToListAsync();
            ViewBag.Languages = await _context.CourseLanguages.Where(p => !p.IsDeleted).ToListAsync();
            ViewBag.CourseAssests = await _context.CourseAssests.Where(p => !p.IsDeleted).ToListAsync();
            Course? course = await _context.Courses.Where(x => x.Id == id && !x.IsDeleted)
                .AsNoTracking()
                .Include(x=>x.courseAssests)
                .Include(x=>x.courseCategories)
                   .ThenInclude(x=>x.Category)
                 .Include(x=>x.courseTags)
                    .ThenInclude(x=>x.Tag)
                  .Include(x=>x.CourseLanguage)
             .FirstOrDefaultAsync();
            if (course is null)
            {
                return NotFound();
            }
            return View(course);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, Course course)
        {
            ViewBag.Categories = await _context.Categories.Where(p => !p.IsDeleted).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(p => !p.IsDeleted).ToListAsync();
            ViewBag.Languages = await _context.CourseLanguages.Where(p => !p.IsDeleted).ToListAsync();
            ViewBag.CourseAssests = await _context.CourseAssests.Where(p => !p.IsDeleted).ToListAsync();
            Course? updatedCourse = await _context.Courses.Where(x => x.Id == id && !x.IsDeleted)
             .AsNoTracking()
             .Include(x => x.courseAssests)
             .Include(x => x.courseCategories)
                .ThenInclude(x => x.Category)
              .Include(x => x.courseTags)
                 .ThenInclude(x => x.Tag)
               .Include(x => x.CourseLanguage)
          .FirstOrDefaultAsync();
            if (course is null)
            {
                return View(course);
            }
            if (!ModelState.IsValid)
            {
                return View(updatedCourse);
            }

            if (course.file is not null)
            {
                if (!Helper.isImage(course.file))
                {
                    ModelState.AddModelError("file", "File must be image");
                    return View(course);
                }
                if (!Helper.isSizeOk(course.file, 1))
                {
                    ModelState.AddModelError("file", "Size of Image must less than 1 mb!!!");
                    return View(course);
                }
                Helper.RemoveImage(_environment.WebRootPath, "img/course/", updatedCourse.Image);
                course.Image = course.file.CreateImage(_environment.WebRootPath, "img/course/");
            }
            else
            {
                course.Image = updatedCourse.Image;
			}
            List<CourseCategory> RemoveableCategory = await _context.CourseCategories.
               Where(x => !course.CategoryIds.Contains(x.CategoryId) && x.CourseId == course.Id ).ToListAsync();

            _context.CourseCategories.RemoveRange(RemoveableCategory);
            foreach (var item in course.CategoryIds)
            {
                if (_context.CourseCategories.Where(x => x.CourseId == id && x.CategoryId == item).Count() > 0)
                    continue;

                await _context.CourseCategories.AddAsync(new CourseCategory
                {
                    CourseId = id,
                    CategoryId = item
                });
            }
            List<CourseTag> RemoveableTag = await _context.CourseTags.
            Where(x => !course.TagIds.Contains(x.TagId) && x.CourseId == course.Id).ToListAsync();

            _context.CourseTags.RemoveRange(RemoveableTag);
            foreach (var item in course.TagIds)
            {
                if (_context.CourseTags.Where(x => x.CourseId == id && x.TagId == item).Count() > 0)
                    continue;

                await _context.CourseTags.AddAsync(new CourseTag
                {
                    CourseId = id,
                    TagId = item
                });
            }
            //updatedCourse.CourseFee = course.CourseFee;
            //updatedCourse.Name = course.Name;
            //updatedCourse.Apply = course.Apply;
            //updatedCourse.Description = course.Description;
            //updatedCourse.AboutText = course.AboutText;
            //updatedCourse.Certificiation = course.Certificiation;
            //updatedCourse.ClassDuration = course.ClassDuration;
            //updatedCourse.StartDate = course.StartDate;
            //updatedCourse.StudentCount = course.StudentCount;
            //updatedCourse.SkillLevel = course.SkillLevel;
            //updatedCourse.EndDate = course.EndDate;
            updatedCourse.UpdatedDate = DateTime.Now;
            updatedCourse.CourseLanguageId = course.CourseLanguageId;
            //updatedCourse.CourseLanguage = course.CourseLanguage;
			updatedCourse.CourseAssestsId = course.CourseAssestsId;
            _context.Courses.Update(course);
			await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Remove(int id)
        {
            Course? course = await _context.Courses.Where(x => x.Id == id && !x.IsDeleted)
                .FirstOrDefaultAsync();
            if (course is null)
            {
                return NotFound();
            }
            course.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
