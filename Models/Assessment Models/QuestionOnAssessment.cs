using TrackFinder.Models.Assessment_Models;

namespace TrackFinder.Models.AssessmentModels
{
    public class QuestionOnAssessment
    {
        public int Id { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public virtual ICollection<TrackQuestionsWeights>? QuestionWeights { get; set; }
    }
}
