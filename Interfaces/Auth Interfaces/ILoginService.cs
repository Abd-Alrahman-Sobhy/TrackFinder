using TrackFinder.ViewModels.Auth_ViewModels;


namespace TrackFinder.Services.AuthServices.Interfaces
{
    /// <summary>
    /// Handles all Login, Logout, and Refresh Token operations.
    /// All business logic lives here — controllers stay thin.
    /// </summary>
    public interface ILoginService
    {
        /// <summary>
        /// Validates credentials and, on success, signs the user in via Cookie Authentication.
        /// This sets the auth cookie + refresh token cookie on the HttpContext response.
        /// After this call, User.Identity.IsAuthenticated == true on subsequent requests.
        /// </summary>
        Task<AuthResultVM> LoginAsync(LoginVM dto, HttpContext httpContext);

        /// <summary>
        /// Signs the user out, clears auth cookie
        /// </summary>
        Task LogoutAsync(HttpContext httpContext);

        /// <summary>
        /// Reads the refresh token from the HttpOnly cookie, validates it against the DB,
        /// and silently re-issues a new auth cookie + new refresh token if valid.
        /// Called automatically when the auth cookie has expired.
        /// </summary>
        Task<AuthResultVM> RefreshTokenAsync(HttpContext httpContext);
    }
}
