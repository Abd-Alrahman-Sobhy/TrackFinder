using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using TrackFinder.Models.CourseModels;

namespace TrackFinder.ViewModels.Courses
{
    public class CourseListVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public Level Level { get; set; }
        public string? InstructorName { get; set; }
    }

    public class CreateCourseVM
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Range(0, int.MaxValue)]
        public int Price { get; set; }
        public IFormFile? Image { get; set; }
        [Required]
        public Language Language { get; set; }
        [Required]
        public Level Level { get; set; }
        public double Rating { get; set; }
        public int DurationValue { get; set; }
        public DurationIn DurationIn { get; set; }
        public double? Discount { get; set; }
        public Guid InstructorId { get; set; }
        public int? TrackStackId { get; set; }
    }

    public class EditCourseVM
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Range(0, int.MaxValue)]
        public decimal Price { get; set; }
        public IFormFile? Image { get; set; }
        [Required]
        public Language Language { get; set; }
        [Required]
        public Level Level { get; set; }
        public double Rating { get; set; }
        public int DurationValue { get; set; }
        public DurationIn DurationIn { get; set; }
        public double? Discount { get; set; }
        public Guid InstructorId { get; set; }
        public int? TrackStackId { get; set; }
    }
    public class CourseDetailsVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public Language Language { get; set; }
        public Level Level { get; set; }
        public double Rating { get; set; }
        public int DurationValue { get; set; }
        public DurationIn DurationIn { get; set; }
        public double? Discount { get; set; }
        public string? InstructorName { get; set; }
        public string? TrackStackName { get; set; }
    }

    // ViewModel-suffixed aliases for Razor Views
    public class CourseListViewModel : CourseListVM { }
    public class CreateCourseViewModel : CreateCourseVM { }
    public class EditCourseViewModel : EditCourseVM { }
    public class CourseDetailsViewModel : CourseDetailsVM { }
}
