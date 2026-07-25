using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TrackFinder.Services.AuthServices.Interfaces;
using TrackFinder.ViewModels.Auth_ViewModels;

namespace TrackFinder.Controllers
{
    [EnableRateLimiting("Auth")]
    public class LoginController : Controller
    {
        private readonly ILoginService         _loginService;
        private readonly IOtpService           _otpService;
        private readonly IPasswordResetService _passwordResetService;

        public LoginController(
            ILoginService loginService,
            IOtpService otpService,
            IPasswordResetService passwordResetService)
        {
            _loginService         = loginService;
            _otpService           = otpService;
            _passwordResetService = passwordResetService;
        }

        // ── GET /Login ────────────────────────────────────────────────────
        [HttpGet]
        public IActionResult Index()
        {
            // If already logged in, skip the login page
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Main");

            return View();
        }

        // ── POST /Login ───────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginVM dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _loginService.LoginAsync(dto, HttpContext);

            // Unverified email → send user straight to the OTP page
            if (result.RequiresEmailVerification)
                return RedirectToAction(nameof(VerifyOtp), new { email = result.Email });

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage!);
                return View(dto);
            }

            return Redirect(result.RedirectUrl!);
        }

        // ── POST /Login/Logout ────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _loginService.LogoutAsync(HttpContext);
            return RedirectToAction(nameof(Index));
        }

        // ── GET /Login/VerifyOtp?email=... ────────────────────────────────
        [HttpGet]
        public IActionResult VerifyOtp(string email)
        {
            if (string.IsNullOrEmpty(email))
                return RedirectToAction(nameof(Index));

            var dto = new VerifyOtpVM { Email = email };
            ViewBag.Email = email;
            return View(dto);
        }

        // ── POST /Login/VerifyOtp ─────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyOtp(VerifyOtpVM dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Email = dto.Email;
                return View(dto);
            }

            var result = await _otpService.VerifyOtpAsync(dto);

            if (!result.Success)
            {
                ViewBag.Email = dto.Email;
                ModelState.AddModelError(string.Empty, result.ErrorMessage!);
                return View(dto);
            }

            TempData["SuccessMessage"] = result.SuccessMessage;
            return Redirect(result.RedirectUrl!);
        }

        // ── POST /Login/ResendOtp ─────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendOtp(string email)
        {
            var result = await _otpService.ResendOtpAsync(email);
            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] =
                result.Success ? result.SuccessMessage : result.ErrorMessage;

            return RedirectToAction(nameof(VerifyOtp), new { email });
        }

        // ── GET /Login/PendingApproval ────────────────────────────────────
        [HttpGet]
        public IActionResult PendingApproval()
        {
            return View();
        }

        // ── POST /Login/Refresh ───────────────────────────────────────────
        // Called by a middleware/filter when the auth cookie has expired
        [HttpPost]
        public async Task<IActionResult> Refresh()
        {
            var result = await _loginService.RefreshTokenAsync(HttpContext);
            if (!result.Success)
                return RedirectToAction(nameof(Index));

            return Redirect(result.RedirectUrl!);
        }

        // ══════════════════════════════════════════════════════════════════
        //  FORGOT PASSWORD — Step 1 : Enter Email → receive OTP
        // ══════════════════════════════════════════════════════════════════

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _passwordResetService.SendResetOtpAsync(dto.Email);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage!);
                return View(dto);
            }

            TempData["SuccessMessage"] = result.SuccessMessage;
            return Redirect(result.RedirectUrl!);
        }

        // ══════════════════════════════════════════════════════════════════
        //  FORGOT PASSWORD — Step 2 : Verify OTP → receive reset token
        // ══════════════════════════════════════════════════════════════════

        [HttpGet]
        public IActionResult ResetPasswordOtp(string email)
        {
            if (string.IsNullOrEmpty(email))
                return RedirectToAction(nameof(ForgotPassword));

            ViewBag.Email = email;
            return View(new ResetPasswordOtpVM { Email = email });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPasswordOtp(ResetPasswordOtpVM dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Email = dto.Email;
                return View(dto);
            }

            var result = await _passwordResetService.VerifyResetOtpAsync(dto);

            if (!result.Success)
            {
                ViewBag.Email = dto.Email;
                ModelState.AddModelError(string.Empty, result.ErrorMessage!);
                return View(dto);
            }

            TempData["SuccessMessage"] = result.SuccessMessage;
            return Redirect(result.RedirectUrl!);
        }

        // ══════════════════════════════════════════════════════════════════
        //  FORGOT PASSWORD — Step 3 : Enter new password → reset
        // ══════════════════════════════════════════════════════════════════

        [HttpGet]
        public IActionResult SetNewPassword(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
                return RedirectToAction(nameof(ForgotPassword));

            ViewBag.Email = email;
            ViewBag.Token = token;
            return View(new SetNewPasswordVM { Email = email, Token = token });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetNewPassword(SetNewPasswordVM dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Email = dto.Email;
                ViewBag.Token = dto.Token;
                return View(dto);
            }

            var result = await _passwordResetService.ResetPasswordAsync(dto);

            if (!result.Success)
            {
                ViewBag.Email = dto.Email;
                ViewBag.Token = dto.Token;
                ModelState.AddModelError(string.Empty, result.ErrorMessage!);
                return View(dto);
            }

            TempData["SuccessMessage"] = result.SuccessMessage;
            return Redirect(result.RedirectUrl!);
        }

        // ══════════════════════════════════════════════════════════════════
        //  FORGOT PASSWORD — Resend the reset OTP (same step as #1)
        // ══════════════════════════════════════════════════════════════════

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendPasswordResetOtp(string email)
        {
            var result = await _passwordResetService.ResendResetOtpAsync(email);
            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] =
                result.Success ? result.SuccessMessage : result.ErrorMessage;

            return RedirectToAction(nameof(ResetPasswordOtp), new { email });
        }
    }
}
