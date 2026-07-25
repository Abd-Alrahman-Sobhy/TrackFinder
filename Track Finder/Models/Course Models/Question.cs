namespace TrackFinder.Models.CourseModels
{
    public enum QuestionType
    {
        MultipleChoice = 1,
        TrueFalse
    }
    public class Question
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string QuestionText { get; set; } = string.Empty;
        public int Points { get; set; }
        public int QuestionNumber { get; set; }
        public QuestionType QuestionType { get; set; } = QuestionType.MultipleChoice;

        public Guid ExamId { get; set; }
        public Exam Exam { get; set; } = new Exam();

        public virtual ICollection<Option>? Options { get; set; } = null!;
        public virtual ICollection<StudentAnswer>? StudentAnswers { get; set; } = null!;
    }
}