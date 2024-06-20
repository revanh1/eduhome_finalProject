using Microsoft.AspNetCore.Mvc;

namespace EduHome.App.Controllers
{
    public class EventController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }
        public async Task<IActionResult> Detail()
        {
            return View();
        }

    }
}
