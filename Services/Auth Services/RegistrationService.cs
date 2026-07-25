using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using TrackFinder.Context;
using TrackFinder.Models.UserModels;
using TrackFinder.Providers.Common.EmailService;
using TrackFinder.Providers.Common.EmailService.Messages;
using TrackFinder.Providers.Private.AuthProviders;
using TrackFinder.Services.AuthServices.Interfaces;
using TrackFinder.ViewModels.Auth_ViewModels;

namespace TrackFinder.Services.AuthServices.Implementations
{
    public class RegistrationService : IRegistrationService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly ITokenProvider _tokenProvider;
        private readonly IEmailSender _emailSender;
        private readonly IMemoryCache _cache;
        private readonly AppDbContext _db;
        private readonly ILogger<RegistrationService> _logger;
        private const int OtpExpiryMinutes = 10;

        public RegistrationService(
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            ITokenProvider tokenProvider,
            IEmailSender emailSender,
            IMemoryCache cache,
            AppDbContext db,
            ILogger<RegistrationService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenProvider = tokenProvider;
            _emailSender = emailSender;
            _cache = cache;
            _db = db;
            _logger = logger;
        }

        private static string CacheKey(string email) => $"otp:{email.ToLower()}";

        public async Task<AuthResultVM> RegisterAsync(RegisterVM dto, string webRootPath)
        {
            var ExistUserEmail = await _userManager.FindByEmailAsync(dto.Email);

            if (ExistUserEmail is not null)
                return AuthResultVM.Fail("Email is already registered. Please use a different email.");

            var ExistUserName = await _userManager.FindByNameAsync(dto.Username);

            if (ExistUserName is not null)
                return AuthResultVM.Fail("Username is already taken. Please choose a different username.");

            // ── 1. Instructor: validate and save CV ───────────────────────
            string? cvFilePath = null;
            bool isInstructor = string.Equals(dto.Role, "Instructor", StringComparison.OrdinalIgnoreCase);

            if (isInstructor)
            {
                if (dto.CvFile is null || dto.CvFile.Length == 0)
                    return AuthResultVM.Fail("A CV file (PDF) is required for instructor registration.");

                var extension = Path.GetExtension(dto.CvFile.FileName).ToLowerInvariant();
                if (extension != ".pdf")
                    return AuthResultVM.Fail("Only PDF files are accepted for your CV.");

                if (dto.CvFile.Length > 5 * 1024 * 1024)
                    return AuthResultVM.Fail("CV file must not exceed 5 MB.");

                var uploadsDir = Path.Combine(webRootPath, "uploads", "InstructorsCVs");
                Directory.CreateDirectory(uploadsDir);

                var fileName = $"{Guid.NewGuid()}{extension}";
                var fullPath = Path.Combine(uploadsDir, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                    await dto.CvFile.CopyToAsync(stream);

                cvFilePath = $"/uploads/InstructorsCVs/{fileName}";
            }

            // ── 2. Build and create the User via Identity ──────────────────
            var user = new User
            {
                UserName = dto.Username,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Gender = dto.Gender,
                Birthdate = dto.Birthdate,
            };

            var createResult = await _userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
            {
                var errorMessage = string.Join(" ", createResult.Errors.Select(e => e.Description));
                return AuthResultVM.Fail(errorMessage);
            }

            // ── 3. Ensure role exists, then assign it ──────────────────────
            var roleName = isInstructor ? "Instructor" : "Student";
            if (!await _roleManager.RoleExistsAsync(roleName))
                await _roleManager.CreateAsync(new Role { Name = roleName });

            await _userManager.AddToRoleAsync(user, roleName);

            // ── 4. Create role-specific profile row ────────────────────────
            if (isInstructor)
            {
                _db.Instructors.Add(new Instructor
                {
                    UserId = user.Id,
                    CvFilePath = cvFilePath,
                    AdminApproved = false   // must be approved by admin before login
                });
            }
            else
            {
                _db.Students.Add(new Student
                {
                    UserId = user.Id
                });
            }

            await _db.SaveChangesAsync();

            // ── 5. Generate + cache OTP, send email ─────────────────────────
            var otpCode = _tokenProvider.GenerateOtpCode();
            _cache.Set(CacheKey(dto.Email), otpCode, TimeSpan.FromMinutes(OtpExpiryMinutes));

            _logger.LogInformation("OTP for {Email}: {OtpCode}", dto.Email, otpCode);

            var emailBody = OtpMessage.Build(dto.FirstName, otpCode, OtpExpiryMinutes);
            _emailSender.SendEmail(dto.Email, emailBody, "Verify your Track Finder account");

            // ── 6. Redirect to OTP verification page ────────────────────────
            var encodedEmail = Uri.EscapeDataString(dto.Email);
            return AuthResultVM.Ok(
                $"/Login/VerifyOtp?email={encodedEmail}",
                "Registration successful! Please check your email for the verification code.");
        }
    }
}