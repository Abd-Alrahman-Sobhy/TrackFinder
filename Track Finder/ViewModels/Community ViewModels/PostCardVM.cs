using TrackFinder.Models.CommunityModels;
using TrackFinder.Models.UserModels;

namespace TrackFinder.ViewModels.Community_ViewModels
{
    public class PostCardVM
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public PostStatus Status { get; set; }
        public int ReportsCount { get; set; }
        public int PostPriority { get; set; }
        public User Author { get; set; } = null!;
        public Community Community { get; set; } = null!;
        public IReadOnlyList<CommentPreviewVM> Comments { get; set; } = [];
        public IReadOnlyList<string> Tags { get; set; } = [];
        public bool CanManage { get; set; }
    }
}
