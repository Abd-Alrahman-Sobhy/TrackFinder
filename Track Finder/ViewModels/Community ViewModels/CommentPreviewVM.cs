using TrackFinder.Models.UserModels;

namespace TrackFinder.ViewModels.Community_ViewModels
{
    public class CommentPreviewVM
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public User? User { get; set; }
        public bool CanManage { get; set; }
    }
}
