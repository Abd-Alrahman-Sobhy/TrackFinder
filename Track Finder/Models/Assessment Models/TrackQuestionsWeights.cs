using TrackFinder.Models.AssessmentModels;

namespace TrackFinder.Models.Assessment_Models
{
    public class TrackQuestionsWeights
    {
        public double Weight { get; set; }

        public int TrackId { get; set; }
        public virtual Track? RelatedTrack { get; set; }

        public int QuestionId { get; set; }
        public virtual QuestionOnAssessment? RelatedQuestion { get; set; }
    }
}