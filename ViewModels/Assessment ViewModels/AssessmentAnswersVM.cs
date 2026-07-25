namespace TrackFinder.ViewModels.Assessment_ViewModels
{
    public class Answer
    {
        public int QuestionId { get; set; }
        public int OptionNumber { get; set; }
    }
    public class AssessmentAnswersVM
    {
        public Guid UserId { get; set; }
        public List<Answer> Answers { get; set; }
    }
}
