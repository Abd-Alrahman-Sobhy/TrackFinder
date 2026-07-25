namespace TrackFinder.Models.CourseModels
{
    public class ExamAttempt
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int Score { get; set; }
        public int RemainingAttempts { get; set; }
        public DateTime AttemptDate { get; set; } = DateTime.UtcNow;

        public Guid ExamId { get; set; }
        public virtual Exam Exam { get; set; } = null!;
        public virtual ICollection<StudentAnswer> StudentAnswers { get; set; } = null!;
    }
}