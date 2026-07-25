using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrackFinder.Models.CourseModels;
using TrackFinder.Models.UserModels;

namespace TrackFinderDb.Models.TeachingModels
{
    public enum EnrollmentStatus
    {
        NotStarted = 1,
        InProgress,
        Completed,
        Dropped
    }
    public class Enrollment
    {
        public int Progress { get; set; }
        public EnrollmentStatus Status { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public DateTime? LastActiveDate { get; set; }
        public int CompletedLessons { get; set; }

        public Guid CourseId { get; set; }
        public Guid UserId { get; set; }
        public virtual Course Course { get; set; } = new Course();
        public virtual User User { get; set; } = new User();

        [NotMapped]
        public string? CardNumber { get; set; }

        [NotMapped]
        public DateTime? ExpiryDate { get; set; }

        [NotMapped]
        public string? CVV { get; set; }
    }
}