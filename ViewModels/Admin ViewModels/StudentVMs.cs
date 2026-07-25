using System.ComponentModel.DataAnnotations;
using TrackFinder.Models.UserModels;

namespace TrackFinder.ViewModels.Students
{
    public class StudentListVM
    {
        public Guid UserId { get; set; }

        public string StudentName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? ProfilePictureUrl { get; set; }

        public string? SchoolOrUnversityName { get; set; }

        public string? Major { get; set; }

        public int AcademicYear { get; set; }

        public float GPA { get; set; }
    }

    public class StudentDetailsVM
    {
        public Guid UserId { get; set; }

        // بيانات User
        public string StudentName { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public DateTime Birthdate { get; set; }

        public Gender Gender { get; set; }

        public string? ProfilePictureUrl { get; set; }

        // بيانات Student
        public string? EducationState { get; set; }

        public string? SchoolOrUnversityName { get; set; }

        public string? Major { get; set; }

        public string? Minor { get; set; }

        public string? DegreeProgram { get; set; }

        public int AcademicYear { get; set; }

        public float GPA { get; set; }

        public string? Bio { get; set; }
    }

    public class CreateStudentVM
    {
        [Required]
        public Guid UserId { get; set; }

        public string? EducationState { get; set; }

        public string? SchoolOrUnversityName { get; set; }

        public string? Major { get; set; }

        public string? Minor { get; set; }

        public string? DegreeProgram { get; set; }

        public int AcademicYear { get; set; }

        public float GPA { get; set; }

        public string? Bio { get; set; }
    }

    public class EditStudentVM : CreateStudentVM
    {
    }

    // ViewModel-suffixed aliases for Razor Views
    public class StudentListViewModel : StudentListVM { }
    public class StudentDetailsViewModel : StudentDetailsVM { }
    public class CreateStudentViewModel : CreateStudentVM { }
    public class EditStudentViewModel : EditStudentVM { }
}