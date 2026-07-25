using System.Security.Claims;
using TrackFinder.Models.UserModels;

namespace TrackFinder.Providers.Private.AuthProviders
{
    /// <summary>
    /// Handles all token and OTP generation:
    ///   - Building the ClaimsPrincipal that ASP.NET Core Cookie Auth encrypts into the cookie
    ///   - Generating cryptographically secure refresh tokens
    ///   - Generating 6-digit OTP codes for email verification
    /// </summary>
    public interface ITokenProvider
    {
        /// <summary>
        /// Generates a 6-digit numeric OTP code for email verification.
        /// </summary>
        string GenerateOtpCode();
    }
}
