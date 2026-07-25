namespace TrackFinder.ViewModels.Assessment_ViewModels
{
    public class TrackStackResultVM
    {
        public int Id { get; set; }
        public string StackName { get; set; } = string.Empty;
    }

    public class AssessmentResultTracksVM
    {
        public int TrackId { get; set; }
        public string TrackName { get; set; } = string.Empty;
        public string TrackDescription { get; set; } = string.Empty;
        public int SimilarityScore { get; set; }
        public string? AverageSalary { get; set; }
        public string? RoadMapUrl { get; set; }
        public List<string> Skills { get; set; } = new();
        public List<TrackStackResultVM> Stacks { get; set; } = new();
    }
}