using TrackFinder.Models.CourseModels;
using TrackFinder.Models.UserModels;

namespace TrackFinder.Models.AchievementModels
{
    public class StudentCertificate
    {
        public string CredentialsId { get; set; } = null!;

        public Guid CourseId { get; set; }
        public int CertificateId { get; set; }
        public Guid StudentId { get; set; }

        // حقول جديدة
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
        public bool IsFeatured { get; set; } = false;

        public virtual Course Course { get; set; } = null!;
        public virtual Certificate Certificate { get; set; } = null!;
        public virtual Student Student { get; set; } = null!;
    }
}