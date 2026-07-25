namespace TrackFinder.ViewModels.Assessment_ViewModels
{
    public class AssessmentHistoryItemVM
    {
        public Guid AssessmentResultId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? TopTrackName { get; set; }
        public string RecommendedTracks { get; set; } = string.Empty;
        public List<string> SimilarityScores { get; set; } = new();
    }
}