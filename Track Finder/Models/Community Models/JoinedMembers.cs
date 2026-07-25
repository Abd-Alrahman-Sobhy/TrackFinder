using TrackFinder.Models.UserModels;

namespace TrackFinder.Models.CommunityModels
{
    public class JoinedMembers
    {
        public Guid MemberId { get; set; }
        public Guid CommunityId { get; set; }
        public virtual Student Member { get; set; } = null!;
        public virtual Community Community { get; set; } = null!;
    }
}
