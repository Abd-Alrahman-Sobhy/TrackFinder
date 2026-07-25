using System.ComponentModel.DataAnnotations;
using TrackFinder.Models.UserModels;

namespace TrackFinder.ViewModels.Instructors
{
    public class CreateInstructorVM
    {
        [Required]
        public Guid UserId { get; set; }
        public string? Title { get; set; }
        public string? GithubLink { get; set; }
        public string? LinkedInLink { get; set; }
        public double Rating { get; set; }
        public Guid? CommunityId { get; set; }
    }

    public class EditInstructorVM : CreateInstructorVM
    {
        public new Guid UserId { get; set; }
    }
    public class InstructorDetailsVM
    {
        public Guid UserId { get; set; }

        public string InstructorName { get; set; } = string.Empty;

        public string? Title { get; set; }

        public string? GithubLink { get; set; }

        public string? LinkedInLink { get; set; }

        public double Rating { get; set; }
    }

    // ViewModel-suffixed aliases for Razor Views
    public class CreateInstructorViewModel : CreateInstructorVM { }
    public class EditInstructorViewModel : EditInstructorVM { }
    public class InstructorDetailsViewModel : InstructorDetailsVM { }
    public class InstructorListViewModel : InstructorDetailsVM { }
}
