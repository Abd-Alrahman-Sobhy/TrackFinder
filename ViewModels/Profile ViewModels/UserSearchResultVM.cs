namespace TrackFinder.ViewModels.Profile_ViewModels
{
    public class UserSearchResultVM
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string Role { get; set; } = string.Empty;
    }
}
