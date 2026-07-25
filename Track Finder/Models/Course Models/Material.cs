namespace TrackFinder.Models.CourseModels
{
    public class Material
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;

        public Guid LessonId { get; set; }
        public Guid CourseId { get; set; }
        public virtual Lesson? Lesson { get; set; }
        public virtual Course? Course { get; set; }
    }
}