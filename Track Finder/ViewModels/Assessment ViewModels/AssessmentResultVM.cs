namespace TrackFinder.ViewModels.Assessment_ViewModels
{
    public class AssessmentResultVM
    {
        public Guid AssessmentResultId { get; set; }
        public List<AssessmentResultTracksVM> Tracks { get; set; }
        public DateTime at { get; set; }
    }
}