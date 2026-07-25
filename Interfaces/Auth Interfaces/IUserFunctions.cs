using TrackFinder.Models.UserModels;

namespace TrackFinder.Services.AuthServices.Helpers
{
    /// <summary>
    /// Low-level DB query helpers shared across auth services.
    /// All methods are async to keep the auth pipeline non-blocking.
    /// </summary>
    public interface IUserFunctions
    {
        // ── Existence Checks ──────────────────────────────────────────────

        /// <summary>Returns true if a user with this email already exists.</summary>
        Task<bool> IsEmailTakenAsync(string email);

        /// <summary>Returns true if a user with this username already exists.</summary>
        Task<bool> IsUsernameTakenAsync(string username);

        /// <summary>Returns true if the user's email has been verified via OTP.</summary>
        Task<bool> IsEmailVerifiedAsync(string email);

        /// <summary>Returns true if the user account is banned.</summary>
        Task<bool> IsUserBannedAsync(string email);

        // ── Retrieval ─────────────────────────────────────────────────────

        /// <summary>
        /// Returns the full User entity by email, or null if not found.
        /// Includes navigation properties needed by auth (Student, Instructor).
        /// </summary>
        Task<User?> GetUserByEmailAsync(string email);
    }
}
