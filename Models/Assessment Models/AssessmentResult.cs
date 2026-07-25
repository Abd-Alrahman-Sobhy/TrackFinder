using TrackFinder.Models.UserModels;

namespace TrackFinder.Models.AssessmentModels
{
    public class AssessmentResult
    {
        public Guid AssessmentResultId { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid UserId { get; set; }
        public virtual Student Student { get; set; } = null!;
        public virtual ICollection<AssessmentResultTracks> RecommendedTracks { get; set; } = null!;
    }
   
}
