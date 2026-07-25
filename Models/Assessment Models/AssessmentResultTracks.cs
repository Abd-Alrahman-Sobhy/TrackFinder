namespace TrackFinder.Models.AssessmentModels
{
    public class AssessmentResultTracks
    {
        public int SimilarityScore { get; set; }

        public Guid AssessmentResultId { get; set; }
        public virtual AssessmentResult AssessmentResult { get; set; } = null!;
        public int TrackId { get; set; }
        public virtual Track Track { get; set; } = null!;
    }
}
