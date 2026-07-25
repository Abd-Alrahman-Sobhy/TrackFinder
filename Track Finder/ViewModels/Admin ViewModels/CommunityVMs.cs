using System.ComponentModel.DataAnnotations;

namespace TrackFinder.ViewModels.Communities
{
    public class CreateCommunityVM
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public Guid AdminId { get; set; }
    }

    public class EditCommunityVM : CreateCommunityVM
    {
        public Guid Id { get; set; }
    }

    public class CommunityListVM
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? AdminName { get; set; }

        public int MemberCount { get; set; }
    }

    public class CommunityDetailsVM
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? AdminName { get; set; }

        public int MemberCount { get; set; }
    }

    // ViewModel-suffixed aliases for Razor Views
    public class CreateCommunityViewModel : CreateCommunityVM { }
    public class EditCommunityViewModel : EditCommunityVM { }
    public class CommunityListViewModel : CommunityListVM { }
    public class CommunityDetailsViewModel : CommunityDetailsVM { }
}