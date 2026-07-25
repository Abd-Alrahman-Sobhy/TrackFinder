namespace TrackFinder.ViewModels.Profile_ViewModels
{
    public class PublicProfileVM
    {
        public string UserName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; }
        public DateTime Birthdate { get; set; }
        public string? Gender { get; set; }
        public string? Bio { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string Role { get; set; } = "Student";

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
