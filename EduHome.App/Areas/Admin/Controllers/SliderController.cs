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

    public class SliderController : Controller
    {
        private readonly EduHomeDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public SliderController(EduHomeDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Slider> sliders = await _context.Sliders.Where(x => !x.IsDeleted)
                 .ToListAsync();
            return View(sliders);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Slider slider)
        {
            if (!ModelState.IsValid)
            {
                return View(slider);
            }
            if(slider.file is null)
            {
                ModelState.AddModelError("file", "Image must be added");
                return View(slider);
            }
            if(!Helper.isImage(slider.file))
            {
                ModelState.AddModelError("file", "File must be image");
                return View(slider);
            }
            if (!Helper.isSizeOk(slider.file,1))
            {
                ModelState.AddModelError("file", "Size of Image must less than 1 mb!!!");
                return View(slider);
            }
            slider.Image = slider.file.CreateImage(_environment.WebRootPath, "img/slider/");
            slider.CreatedDate = DateTime.Now;
            await _context.AddAsync(slider);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            Slider? slider = await _context.Sliders.Where(x => x.Id == id && !x.IsDeleted)
             .FirstOrDefaultAsync();
            if (slider is null)
            {
                return NotFound();
            }
            return View(slider);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id,Slider slider)
        {
            Slider? updatedSlider = await _context.Sliders.Where(x => x.Id == id && !x.IsDeleted)
                  .FirstOrDefaultAsync();
            if(slider is null)
            {
                return View(slider);
            }
            if (!ModelState.IsValid)
            {
                return View(updatedSlider);
            }

            if(slider.file is not null)
            {
                if (!Helper.isImage(slider.file))
                {
                    ModelState.AddModelError("file", "File must be image");
                    return View(slider);
                }
                if (!Helper.isSizeOk(slider.file, 1))
                {
                    ModelState.AddModelError("file", "Size of Image must less than 1 mb!!!");
                    return View(slider);
                }
                Helper.RemoveImage(_environment.WebRootPath, "img/slider/", updatedSlider.Image);
                updatedSlider.Image = slider.file.CreateImage(_environment.WebRootPath, "img/slider/");
            }
            updatedSlider.Title = slider.Title;
            updatedSlider.Text = slider.Text;
            updatedSlider.Link = slider.Link;
            updatedSlider.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Remove(int id)
        {
            Slider? slider = await _context.Sliders.Where(x=>x.Id ==id && !x.IsDeleted)
                .FirstOrDefaultAsync();
            if(slider is null)
            {
                return NotFound();
            }
            slider.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
