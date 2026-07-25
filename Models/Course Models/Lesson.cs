namespace TrackFinder.Models.CourseModels
{
    public class LessonDuration
    {
        public int Minutes { get; set; }
        public int Seconds { get; set; }
    }
    public class Lesson
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public LessonDuration Duration { get; set; }
        public int Price { get; set; }
        public double? Discount { get; set; }

        public Guid? CourseId { get; set; }
        public virtual Course? Course { get; set; } = null!;
        public virtual ICollection<Material>? Materials { get; set; } = null!;
        public virtual ICollection<Exam>? Exams { get; set; } = null!;

        public void MapFrom(Lesson lesson)
        {
            Name = lesson.Name;
            Description = lesson.Description;
            Duration = lesson.Duration;
            Price = lesson.Price;
            Discount = lesson.Discount;
        }
    }
}