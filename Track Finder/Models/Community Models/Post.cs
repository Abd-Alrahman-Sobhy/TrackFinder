using TrackFinder.Models.UserModels;

namespace TrackFinder.Models.CommunityModels
{
    public enum PostStatus
    {
        PendingReview = 1,
        Approved,
        Rejected
    }
    public class Post
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? ImageUrl { get; set; }
        public PostStatus Status { get; set; }
        public int ReportsCount { get; set; }
        public int PostPriority { get; set; }

        public Guid UserId { get; set; }
        public Guid GroupId { get; set; }
        public virtual User Author { get; set; } = null!;
        public virtual Community Community { get; set; } = null!;
        public virtual ICollection<Comment> Comments { get; set; } = [];
    }
}
