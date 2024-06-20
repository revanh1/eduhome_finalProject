using EduHome.App.Context;
using EduHome.App.ViewModels;
using EduHome.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace EduHome.App.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class UserController : Controller
    {
        private readonly EduHomeDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public UserController(EduHomeDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            int i = 1;
            var allusers = await _context.Users.
                Skip((page - 1) * 5).Take(5)
                .ToListAsync();
            List<AppUser> users = new List<AppUser>();
            foreach(var user in allusers)
            {
                if (await _userManager.IsInRoleAsync(user,"User"))
                {
                    users.Add(user);
                    i++;
                }
            }
            int TotalCount = i;
            ViewBag.TotalPage = (int)Math.Ceiling((decimal)TotalCount / 5);
            ViewBag.CurrentPage = page;
            return View(users);
        }
        [HttpGet]
        public async Task<IActionResult> Remove(string id)
        {
            var allusers = await _context.Users.ToListAsync();
            List<AppUser> users = new List<AppUser>();
            foreach (var user in allusers)
            {
                if (await _userManager.IsInRoleAsync(user, "User"))
                {
                    users.Add(user);
                }
            }
            var user1 = users.FirstOrDefault(x => x.Id == id);
            if (user1 is null)
            {
                return NotFound();
            }
            _context.Users.Remove(user1);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
