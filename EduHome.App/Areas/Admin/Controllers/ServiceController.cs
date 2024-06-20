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

    public class ServiceController : Controller
    {
        private readonly EduHomeDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ServiceController(EduHomeDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Service> services = await _context.Services.Where(x => !x.IsDeleted)
                 .ToListAsync();
            return View(services);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Service service)
        {
            if (!ModelState.IsValid)
            {
                return View(service);
            }
            if(service.file is null)
            {
                ModelState.AddModelError("file", "Image must be added");
                return View(service);
            }
            if(!Helper.isImage(service.file))
            {
                ModelState.AddModelError("file", "File must be image");
                return View(service);
            }
            if (!Helper.isSizeOk(service.file,1))
            {
                ModelState.AddModelError("file", "Size of Image must less than 1 mb!!!");
                return View(service);
            }
            service.AboutImage = service.file.CreateImage(_environment.WebRootPath, "img/slider/");
            service.CreatedDate = DateTime.Now;
            await _context.AddAsync(service);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            Service? service = await _context.Services.Where(x => x.Id == id && !x.IsDeleted)
             .FirstOrDefaultAsync();
            if (service is null)
            {
                return NotFound();
            }
            return View(service);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id,Service service)
        {
            Service? updatedService = await _context.Services.Where(x => x.Id == id && !x.IsDeleted)
                  .FirstOrDefaultAsync();
            if(service is null)
            {
                return View(service);
            }
            if (!ModelState.IsValid)
            {
                return View(updatedService);
            }

            if(service.file is not null)
            {
                if (!Helper.isImage(service.file))
                {
                    ModelState.AddModelError("file", "File must be image");
                    return View(service);
                }
                if (!Helper.isSizeOk(service.file, 1))
                {
                    ModelState.AddModelError("file", "Size of Image must less than 1 mb!!!");
                    return View(service);
                }
                Helper.RemoveImage(_environment.WebRootPath, "img/about/",updatedService.AboutImage);
                updatedService.AboutImage = service.file.CreateImage(_environment.WebRootPath, "img/about/");
            }
            updatedService.AboutText = service.AboutText;
            updatedService.AboutTitle = service.AboutTitle;
            updatedService.VideoLink = service.VideoLink;
            updatedService.Address = service.Address;
            updatedService.Number1 = service.Number1;
            updatedService.Number2 = service.Number2;
            updatedService.Email = service.Email;
            updatedService.FooterLogo = service.FooterLogo;
            updatedService.HeaderLogo = service.HeaderLogo;
            updatedService.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Remove(int id)
        {
            Service? service = await _context.Services.Where(x=>x.Id ==id && !x.IsDeleted)
                .FirstOrDefaultAsync();
            if(service is null)
            {
                return NotFound();
            }
            service.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
