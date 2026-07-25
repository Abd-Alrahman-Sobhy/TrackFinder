namespace TrackFinder.Providers.Private.AuthProviders
{
    /// <summary>
    /// Handles all password hashing and verification using BCrypt.
    /// Never deal with plain-text passwords outside this provider.
    /// </summary>
    public interface IEncryptPasswordProvider
    {
        /// <summary>Hashes a plain-text password using BCrypt.</summary>
        string HashPassword(string plainPassword);

        /// <summary>Verifies a plain-text password against a stored BCrypt hash.</summary>
        bool VerifyPassword(string plainPassword, string hashedPassword);
    }
}
