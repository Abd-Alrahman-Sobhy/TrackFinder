namespace TrackFinder.Models.CourseModels
{
    public class Option
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string OptionText { get; set; } = string.Empty;
        public int OptionNumber { get; set; }

        public Guid QuestionId { get; set; }
        public virtual Question Question { get; set; } = null!;

        public virtual ICollection<StudentAnswer>? StudentAnswers { get; set; }
    }
}