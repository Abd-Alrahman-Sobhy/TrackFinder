using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TrackFinder.Context;
using TrackFinder.Models.UserModels;
using TrackFinder.Services.AuthServices.Interfaces;
using TrackFinder.ViewModels.Auth_ViewModels;

namespace TrackFinder.Services.AuthServices.Implementations
{
    public class LoginService : ILoginService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly AppDbContext _db;

        public LoginService(UserManager<User> userManager, SignInManager<User> signInManager, AppDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }

        public async Task<AuthResultVM> LoginAsync(LoginVM dto, HttpContext httpContext)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user is null)
                return AuthResultVM.Fail("Invalid email or password.");

            if (user.IsBanned)
                return AuthResultVM.Fail("This account has been banned.");

            // ── 1. Verify email first ────────────────────────────────────────
            if (!user.EmailConfirmed)
                return AuthResultVM.UnverifiedEmail(user.Email!);

            // ── 2. Validate password ─────────────────────────────────────────
            var result = await _signInManager.PasswordSignInAsync(
                user, dto.Password, isPersistent: false, lockoutOnFailure: true);

            if (result.IsLockedOut)
                return AuthResultVM.Fail("Too many failed attempts. Try again in 15 minutes.");

            if (!result.Succeeded)
                return AuthResultVM.Fail("Invalid email or password.");

            // ── 3. Admin approval gate — only after credentials are valid ────
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Instructor"))
            {
                var instructor = await _db.Instructors
                    .FirstOrDefaultAsync(i => i.UserId == user.Id);

                if (instructor == null || !instructor.AdminApproved)
                {
                    // Sign back out — password was correct but account not approved yet
                    await _signInManager.SignOutAsync();
                    return AuthResultVM.Fail("Your account is pending admin approval. You will be notified once approved.");
                }
            }

            return AuthResultVM.Ok("/Main");
        }

        public async Task LogoutAsync(HttpContext httpContext)
        {
            await _signInManager.SignOutAsync();
        }

        public Task<AuthResultVM> RefreshTokenAsync(HttpContext httpContext)
        {
            // Not needed anymore — cookie auth renews itself via SlidingExpiration.
            return Task.FromResult(AuthResultVM.Fail("Not used with cookie authentication."));
        }
    }
}