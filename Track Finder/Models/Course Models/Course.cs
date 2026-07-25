using System.ComponentModel.DataAnnotations;
using TrackFinder.Models.AssessmentModels;
using TrackFinder.Models.UserModels;
using TrackFinderDb.Models.TeachingModels;

namespace TrackFinder.Models.CourseModels;

public enum Level
{
    Beginner = 1,
    Intermediate,
    Advanced
}
public enum Language
{
    English = 1,
    Arabic
}
public enum DurationIn
{
    Days = 1,
    Weeks,
    Months
}
public class CourseDuration
{
    public int Value { get; set; }
    public DurationIn DurationIn { get; set; }
}
public class Course
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public Language Language { get; set; }
    public Level Level { get; set; }
    [Range(0, 5)]
    public double Rating { get; set; }
    public CourseDuration Duration { get; set; } = null!;
    [Range(0, 100)]
    public double? Discount { get; set; }

    public Guid InstructorId { get; set; }
    public virtual Instructor? Instructor { get; set; }

    public int? TrackStackId { get; set; }
    public virtual TrackStack? TrackStack { get; set; }

    public virtual ICollection<Material>? Materials { get; set; }
    public virtual ICollection<Enrollment>? Enrollments { get; set; }
    public virtual ICollection<Lesson>? Lessons { get; set; }
    public virtual ICollection<CourseSkill>? CourseSkills { get; set; }

    public void MapFrom(Course course)
    {
        Name = course.Name;
        Description = course.Description;
        Price = course.Price;
        ImageUrl = course.ImageUrl;
        Language = course.Language;
        Level = course.Level;
        Rating = course.Rating;
        Duration = course.Duration;
        Discount = course.Discount;
    }
}