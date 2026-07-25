using TrackFinder.Models.UserModels;

namespace TrackFinder.Models.CommunityModels
{
    public class Comment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid PostId { get; set; }
        public Guid? UserId { get; set; }
        public Guid? ParentCommentId { get; set; }
        public virtual Post Post { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual Comment? ParentComment { get; set; }
        public virtual ICollection<Comment>? Replies { get; set; } = [];
    }
}
