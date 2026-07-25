using TrackFinder.Models.AchievementModels;

namespace TrackFinder.Models.ViewModels
{
    public class AchievementVM
    {
        public List<StudentCertificate> StudentCertificates { get; set; } = new();

        public StudentCertificate? FeaturedCertificate { get; set; }

        public List<Badges> AllBadges { get; set; } = new();

        public List<UserBadge> EarnedBadges { get; set; } = new();

        public Certificate? NextMilestoneCertificate { get; set; }
    }
}