using Microsoft.AspNetCore.Identity;
using TrackFinder.Models.AchievementModels;
using TrackFinder.Models.CommunityModels;

namespace TrackFinder.Models.UserModels
{
    public enum UserRole
    {
        Student,
        Instructor,
        Admin
    }

    public enum Gender
    {
        Female,
        Male
    }

    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; }
        public DateTime Birthdate { get; set; }
        public Gender Gender { get; set; }
        public string? Bio { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public bool IsBanned { get; set; } = false;
        public bool EmailVerified { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public UserRole Role { get; set; }

        public virtual Student? Student { get; set; }
        public virtual Instructor? Instructor { get; set; }
        public virtual ICollection<UserBadge>? UserBadges { get; set; }
        public virtual ICollection<Post>? Posts { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<PostReport>? PostReports { get; set; }
    }
}