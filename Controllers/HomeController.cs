using Microsoft.AspNetCore.Mvc;

namespace TrackFinder.Controllers
{
    public class HomeController : Controller
    {
        // ── GET / or /Home ──────────────────────────────────────────────
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // ── GET /Home/Privacy ────────────────────────────────────────────
        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }

        // ── GET /Home/Error ──────────────────────────────────────────────
        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}