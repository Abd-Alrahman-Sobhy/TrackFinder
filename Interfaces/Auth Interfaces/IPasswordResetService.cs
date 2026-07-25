using TrackFinder.ViewModels.Auth_ViewModels;

namespace TrackFinder.Services.AuthServices.Interfaces
{
    /// <summary>
    /// Handles the "Forgot Password" flow — distinct from email-verification OTP.
    ///
    /// The flow has 3 steps:
    ///   1. SendPasswordResetOtpAsync  — user enters email → receives 6-digit OTP
    ///   2. VerifyPasswordResetOtpAsync — user enters OTP → receives a reset token
    ///   3. ResetPasswordAsync          — user enters new password → password updated
    /// </summary>
    public interface IPasswordResetService
    {
        /// <summary>
        /// Step 1 — Generates a 6-digit OTP, caches it under a separate key
        /// ("pwd_reset_otp:{email}"), and emails it to the user.
        /// Returns a redirect to the OTP-verification page.
        /// </summary>
        Task<AuthResultVM> SendResetOtpAsync(string email);

        /// <summary>
        /// Step 2 — Validates the OTP from cache. On success:
        ///   - removes the OTP cache entry
        ///   - generates an ASP.NET Identity password-reset token
        ///   - caches the token under "reset_token:{email}"
        ///   - returns a redirect to the "set new password" page (with ?email=&token=)
        /// </summary>
        Task<AuthResultVM> VerifyResetOtpAsync(ResetPasswordOtpVM dto);

        /// <summary>
        /// Resends the password-reset OTP (overwrites the previous code, resets expiry).
        /// Used when the user clicks "Resend Code" on the OTP page.
        /// </summary>
        Task<AuthResultVM> ResendResetOtpAsync(string email);

        /// <summary>
        /// Step 3 — Validates the cached reset token, then calls
        /// UserManager.ResetPasswordAsync to atomically set the new password.
        /// On success, clears the token cache entry and redirects to /Login.
        /// </summary>
        Task<AuthResultVM> ResetPasswordAsync(SetNewPasswordVM dto);
    }
}
