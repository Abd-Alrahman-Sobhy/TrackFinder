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
    public class PasswordResetService : IPasswordResetService
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenProvider _tokenProvider;
        private readonly IEmailSender _emailSender;
        private readonly IMemoryCache _cache;

        private const int OtpExpiryMinutes = 10;
        private const int TokenExpiryMinutes = 15;

        // Separate cache namespaces so reset OTPs never clash with verification OTPs
        private static string OtpCacheKey(string email) => $"pwd_reset_otp:{email.ToLower()}";
        private static string TokenCacheKey(string email) => $"reset_token:{email.ToLower()}";

        public PasswordResetService(
            UserManager<User> userManager,
            ITokenProvider tokenProvider,
            IEmailSender emailSender,
            IMemoryCache cache)
        {
            _userManager = userManager;
            _tokenProvider = tokenProvider;
            _emailSender = emailSender;
            _cache = cache;
        }

        // ── Step 1: Send OTP ──────────────────────────────────────────────

        public async Task<AuthResultVM> SendResetOtpAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return AuthResultVM.Fail("No account found with that email address.");

            var otpCode = GenerateAndCacheOtp(email);
            SendOtpEmail(user.FirstName, email, otpCode);

            return AuthResultVM.Ok(
                $"/Login/ResetPasswordOtp?email={Uri.EscapeDataString(email)}",
                "A password reset code has been sent to your email.");
        }

        // ── Step 2: Verify OTP → issue reset token ────────────────────────

        public async Task<AuthResultVM> VerifyResetOtpAsync(ResetPasswordOtpVM dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user is null)
                return AuthResultVM.Fail("Account not found.");

            // Validate the OTP from cache
            if (!_cache.TryGetValue(OtpCacheKey(dto.Email), out string? cachedOtp))
                return AuthResultVM.Fail("Your code has expired. Please request a new one.");

            if (cachedOtp != dto.OtpCode)
                return AuthResultVM.Fail("Invalid verification code.");

            // OTP is valid — clean up and issue a reset token
            _cache.Remove(OtpCacheKey(dto.Email));

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            _cache.Set(TokenCacheKey(dto.Email), resetToken, TimeSpan.FromMinutes(TokenExpiryMinutes));

            return AuthResultVM.Ok(
                $"/Login/SetNewPassword?email={Uri.EscapeDataString(dto.Email)}&token={Uri.EscapeDataString(resetToken)}",
                "Code verified! You can now set a new password.");
        }

        // ── Resend (same as step 1, but overwrites the old OTP) ───────────

        public async Task<AuthResultVM> ResendResetOtpAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return AuthResultVM.Fail("No account found with that email address.");

            var otpCode = GenerateAndCacheOtp(email);
            SendOtpEmail(user.FirstName, email, otpCode);

            return AuthResultVM.Ok(
                $"/Login/ResetPasswordOtp?email={Uri.EscapeDataString(email)}",
                "A new password reset code has been sent to your email.");
        }

        // ── Step 3: Set new password ──────────────────────────────────────

        public async Task<AuthResultVM> ResetPasswordAsync(SetNewPasswordVM dto)
        {
            // Server-side confirmation check (client-side via [Compare] attribute too)
            if (dto.NewPassword != dto.ConfirmNewPassword)
                return AuthResultVM.Fail("Passwords do not match.");

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user is null)
                return AuthResultVM.Fail("Account not found.");

            // Validate the reset token from cache
            if (!_cache.TryGetValue(TokenCacheKey(dto.Email), out string? cachedToken))
                return AuthResultVM.Fail("Your password reset session has expired. Please start again.");

            if (cachedToken != dto.Token)
                return AuthResultVM.Fail("Invalid reset session. Please start again.");

            // Atomically set the new password (no intermediate null PasswordHash)
            var resetResult = await _userManager.ResetPasswordAsync(user, cachedToken, dto.NewPassword);
            if (!resetResult.Succeeded)
            {
                var errors = string.Join(" ", resetResult.Errors.Select(e => e.Description));
                return AuthResultVM.Fail(errors);
            }

            _cache.Remove(TokenCacheKey(dto.Email));

            return AuthResultVM.Ok("/Login", "Password reset successfully! You can now log in with your new password.");
        }

        // ── Private helpers ───────────────────────────────────────────────

        private string GenerateAndCacheOtp(string email)
        {
            var otpCode = _tokenProvider.GenerateOtpCode();
            _cache.Set(OtpCacheKey(email), otpCode, TimeSpan.FromMinutes(OtpExpiryMinutes));
            return otpCode;
        }

        private void SendOtpEmail(string firstName, string email, string otpCode)
        {
            var emailBody = ResetPasswordOtpMessage.Build(firstName, otpCode, OtpExpiryMinutes);
            _emailSender.SendEmail(email, emailBody, "Reset your Track Finder password");
        }
    }
}
