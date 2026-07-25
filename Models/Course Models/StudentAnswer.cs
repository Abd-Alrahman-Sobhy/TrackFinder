using TrackFinder.Models.UserModels;

namespace TrackFinder.Models.CourseModels
{
    public class StudentAnswer
    {
        public Guid ExamAttepmtId { get; set; }
        public virtual ExamAttempt? ExamAttempt { get; set; }

        public Guid QuestionId { get; set; }
        public virtual Question? Question { get; set; }

        public Guid AnswerId { get; set; }
        public virtual Option? Answer { get; set; }

        public Guid StudentId { get; set; }
        public virtual Student? Student { get; set; }
    }
}