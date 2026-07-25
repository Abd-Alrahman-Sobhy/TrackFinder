namespace TrackFinder.Providers.Private.AuthProviders
{
    /// <summary>
    /// Implements password hashing and verification using BCrypt.Net-Next.
    /// Work factor 12 gives a good security/performance balance.
    /// </summary>
    public class EncryptPasswordProvider : IEncryptPasswordProvider
    {
        private const int WorkFactor = 12;

        /// <inheritdoc/>
        public string HashPassword(string plainPassword)
        {
            return BCrypt.Net.BCrypt.HashPassword(plainPassword, WorkFactor);
        }

        /// <inheritdoc/>
        public bool VerifyPassword(string plainPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
        }
    }
}
