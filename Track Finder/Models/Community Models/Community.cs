using TrackFinder.Models.UserModels;

namespace TrackFinder.Models.CommunityModels
{
    public class Community
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public Guid AdminId { get; set; }
        public virtual Instructor Admin { get; set; } = null!;
        public virtual ICollection<JoinedMembers> JoinedMembers { get; set; } = null!;
        public virtual ICollection<Post> Posts { get; set; } = null!;
        public virtual ICollection<Instructor>? Moderators { get; set; } = null!;
    }
}
