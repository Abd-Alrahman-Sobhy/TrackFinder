using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using TrackFinder.Models.UserModels;
using TrackFinder.Providers.Common.EmailService;
using TrackFinder.Providers.Common.EmailService.Messages;
using TrackFinder.Providers.Private.AuthProviders;
using TrackFinder.Services.AuthServices.Interfaces;
using TrackFinder.ViewModels.Auth_ViewModels;

namespace TrackFinder.Services.AuthServices.Implementations
{
    public class OtpService : IOtpService
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenProvider _tokenProvider;
        private readonly IEmailSender _emailSender;
        private readonly IMemoryCache _cache;
        private readonly ILogger<OtpService> _logger;
        private const int OtpExpiryMinutes = 10;

        private static string CacheKey(string email) => $"otp:{email.ToLower()}";

        public OtpService(
            UserManager<User> userManager,
            ITokenProvider tokenProvider,
            IEmailSender emailSender,
            IMemoryCache cache,
            ILogger<OtpService> logger)
        {
            _userManager = userManager;
            _tokenProvider = tokenProvider;
            _emailSender = emailSender;
            _cache = cache;
            _logger = logger;
        }

        public async Task<AuthResultVM> VerifyOtpAsync(VerifyOtpVM dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user is null)
                return AuthResultVM.Fail("Account not found.");

            if (!_cache.TryGetValue(CacheKey(dto.Email), out string? cachedOtp))
                return AuthResultVM.Fail("Your code has expired. Please request a new one.");

            if (cachedOtp != dto.OtpCode)
                return AuthResultVM.Fail("Invalid verification code.");

            user.EmailConfirmed = true;
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return AuthResultVM.Fail("Something went wrong verifying your email.");

            _cache.Remove(CacheKey(dto.Email));

            bool isInstructor = await _userManager.IsInRoleAsync(user, "Instructor");

            return isInstructor
                ? AuthResultVM.Ok("/Login/PendingApproval",
                    "Email verified! Your account is now pending admin review.")
                : AuthResultVM.Ok("/Login",
                    "Email verified! You can now log in.");
        }

        public async Task<AuthResultVM> ResendOtpAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return AuthResultVM.Fail("Account not found.");

            if (user.EmailConfirmed)
                return AuthResultVM.Fail("This email is already verified.");

            var otpCode = _tokenProvider.GenerateOtpCode();
            _cache.Set(CacheKey(email), otpCode, TimeSpan.FromMinutes(OtpExpiryMinutes));

            _logger.LogInformation("OTP for {Email}: {OtpCode}", email, otpCode);

            var emailBody = OtpMessage.Build(user.FirstName, otpCode, OtpExpiryMinutes);
            _emailSender.SendEmail(email, emailBody, "Verify your Track Finder account");

            return AuthResultVM.Ok(
                $"/Login/VerifyOtp?email={Uri.EscapeDataString(email)}",
                "A new verification code has been sent to your email.");
        }
    }
}