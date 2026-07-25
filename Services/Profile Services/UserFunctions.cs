using Microsoft.EntityFrameworkCore;
using TrackFinder.Context;
using TrackFinder.Models.UserModels;

namespace TrackFinder.Services.AuthServices.Helpers
{
    /// <summary>
    /// Implements IUserFunctions using EF Core + AppDbContext.
    /// </summary>
    public class UserFunctions : IUserFunctions
    {
        private readonly AppDbContext _db;

        public UserFunctions(AppDbContext db)
        {
            _db = db;
        }

        /// <inheritdoc/>
        public async Task<bool> IsEmailTakenAsync(string email)
            => await _db.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());

        /// <inheritdoc/>
        public async Task<bool> IsUsernameTakenAsync(string username)
            => await _db.Users.AnyAsync(u => u.UserName.ToLower() == username.ToLower());

        /// <inheritdoc/>
        public async Task<bool> IsEmailVerifiedAsync(string email)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
            return user?.EmailConfirmed ?? false;
        }

        /// <inheritdoc/>
        public async Task<bool> IsUserBannedAsync(string email)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
            return user?.IsBanned ?? false;
        }

        /// <inheritdoc/>
        public async Task<User?> GetUserByEmailAsync(string email)
            => await _db.Users
                        .Include(u => u.Student)
                        .Include(u => u.Instructor)
                        .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }
}