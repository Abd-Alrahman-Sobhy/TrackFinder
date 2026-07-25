using System.ComponentModel.DataAnnotations;

namespace TrackFinder.ViewModels.Achievements
{
    public class CreateCertificateVM
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public IFormFile? File { get; set; }
    }

    public class EditCertificateVM : CreateCertificateVM
    {
        public int Id { get; set; }
    }

    public class CertificateListVM
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? CertificateUrl { get; set; }
    }

    public class CertificateDetailsVM
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? CertificateUrl { get; set; }
    }

    public class CreateBadgeVM
    {
        [Required]
        public string BadgeName { get; set; } = string.Empty;
        public string? BadgeDescription { get; set; }
        public IFormFile? Image { get; set; }
    }

    public class EditBadgeVM : CreateBadgeVM
    {
        public int BadgeId { get; set; }
    }

    public class BadgeListVM
    {
        public int BadgeId { get; set; }
        public string BadgeName { get; set; } = string.Empty;
        public string? BadgeDescription { get; set; }
        public string? BadgeImageUrl { get; set; }
    }

    public class BadgeDetailsVM
    {
        public int BadgeId { get; set; }
        public string BadgeName { get; set; } = string.Empty;
        public string? BadgeDescription { get; set; }
        public string? BadgeImageUrl { get; set; }
    }

    // ViewModel-suffixed aliases for Razor Views
    public class CreateCertificateViewModel : CreateCertificateVM { }
    public class EditCertificateViewModel : EditCertificateVM { }
    public class CertificateListViewModel : CertificateListVM { }
    public class CertificateDetailsViewModel : CertificateDetailsVM { }
    public class CreateBadgeViewModel : CreateBadgeVM { }
    public class EditBadgeViewModel : EditBadgeVM { }
    public class BadgeListViewModel : BadgeListVM { }
    public class BadgeDetailsViewModel : BadgeDetailsVM { }
}
