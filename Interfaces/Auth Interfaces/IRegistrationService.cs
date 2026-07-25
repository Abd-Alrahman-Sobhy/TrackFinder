using TrackFinder.ViewModels.Auth_ViewModels;

namespace TrackFinder.Services.AuthServices.Interfaces
{
    /// <summary>
    /// Handles new user registration for both Students and Instructors.
    /// </summary>
    public interface IRegistrationService
    {
        /// <summary>
        /// Registers a new user (Student or Instructor).
        /// On success, generates and emails an OTP for email verification,
        /// then returns a redirect to the OTP verification page.
        /// </summary>
        Task<AuthResultVM> RegisterAsync(RegisterVM dto, string webRootPath);
    }
}
