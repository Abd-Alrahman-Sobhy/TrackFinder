using TrackFinder.ViewModels.Auth_ViewModels;

namespace TrackFinder.Services.AuthServices.Interfaces
{
    /// <summary>
    /// Handles email-verification OTP only.
    /// Password-reset OTP is handled by <see cref="IPasswordResetService"/>.
    /// </summary>
    public interface IOtpService
    {
        Task<AuthResultVM> VerifyOtpAsync(VerifyOtpVM dto);
        Task<AuthResultVM> ResendOtpAsync(string email);
    }
}
