using TrackFinder.Models.UserModels;

namespace TrackFinder.Models.CommunityModels
{
    public enum ReportReason
    {
        Spam,
        Harassment,
        OffensiveContent,
        FakeInformation,
        Other
    }
    public class PostReport
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public ReportReason Reason { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid PostId { get; set; }
        public Guid ReporterId { get; set; }
        public virtual Post Post { get; set; } = null!;
        public virtual User Reporter { get; set; } = null!;
    }
}
