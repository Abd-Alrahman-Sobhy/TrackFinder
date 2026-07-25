using Microsoft.AspNetCore.Mvc;
using TrackFinder.Services.AuthServices.Interfaces;
using TrackFinder.ViewModels.Auth_ViewModels;

namespace TrackFinder.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly IRegistrationService _registrationService;
        private readonly IWebHostEnvironment  _env;

        public RegistrationController(IRegistrationService registrationService, IWebHostEnvironment env)
        {
            _registrationService = registrationService;
            _env                 = env;
        }

        // ── GET /Registration ─────────────────────────────────────────────
        [HttpGet]
        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Main");

            return View(new RegisterVM());
        }

        // ── POST /Registration ────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(RegisterVM dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            dto.ConfirmPassword = dto.Password;
            var result = await _registrationService.RegisterAsync(dto, _env.WebRootPath);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage!);
                return View(dto);
            }

            TempData["SuccessMessage"] = result.SuccessMessage;
            return Redirect(result.RedirectUrl!);
        }
    }
}
