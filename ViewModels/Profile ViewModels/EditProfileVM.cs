using System.ComponentModel.DataAnnotations;

namespace TrackFinder.ViewModels.Profile_ViewModels
{
    public class EditProfileVM
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Birth date is required")]
        [DataType(DataType.Date)]
        public DateTime Birthdate { get; set; }

        [StringLength(20, ErrorMessage = "Gender cannot exceed 20 characters")]
        public string? Gender { get; set; }

        [StringLength(500, ErrorMessage = "Bio cannot exceed 500 characters")]
        public string? Bio { get; set; }

        public IFormFile? ProfilePicture { get; set; }

        public string? ProfilePictureUrl { get; set; }

        public string? EducationState { get; set; }
        public string? SchoolOrUnversityName { get; set; }
        public string? Major { get; set; }
        public string? Minor { get; set; }
        public string? DegreeProgram { get; set; }
        public int AcademicYear { get; set; }
        public float GPA { get; set; }

        public string? Title { get; set; }
        public string? GithubLink { get; set; }
        public string? LinkedInLink { get; set; }
    }
}
