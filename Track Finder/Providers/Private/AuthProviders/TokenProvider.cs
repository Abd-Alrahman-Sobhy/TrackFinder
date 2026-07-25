using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Security.Cryptography;
using TrackFinder.Models.UserModels;

namespace TrackFinder.Providers.Private.AuthProviders
{
    /// <summary>
    /// Concrete implementation of ITokenProvider.
    /// </summary>
    public class TokenProvider : ITokenProvider
    {
        /// <inheritdoc/>
        public string GenerateOtpCode()
        {
            // Cryptographically secure 6-digit code (100000–999999)
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[4];
            rng.GetBytes(bytes);
            var value = Math.Abs(BitConverter.ToInt32(bytes, 0));
            return (100000 + (value % 900000)).ToString();
        }
    }
}
