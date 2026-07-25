using System.ComponentModel.DataAnnotations;
using TrackFinder.Models.AchievementModels;
using TrackFinder.Models.AssessmentModels;
using TrackFinder.Models.CommunityModels;
using TrackFinder.Models.CourseModels;
using TrackFinderDb.Models.TeachingModels;

namespace TrackFinder.Models.UserModels
{
	public class Student
	{
		public string? EducationState { get; set; }
		public string? SchoolOrUnversityName { get; set; }
		public string? Major { get; set; }
		public string? Minor { get; set; }
		public string? DegreeProgram { get; set; }
		public int AcademicYear { get; set; }
		public float GPA { get; set; }
		public string? Bio { get; set; }

		[Key]
		public Guid UserId { get; set; }
		public virtual User User { get; set; } = null!;

		public virtual ICollection<Enrollment>? Enrollments { get; set; }
		public virtual ICollection<AssessmentResult>? AssessmentResults { get; set; }
		public virtual ICollection<StudentCertificate>? AchievedCertificates { get; set; }
		public virtual ICollection<JoinedMembers>? JoinedCommunities { get; set; }
		public virtual ICollection<StudentAnswer>? StudentAnswers { get; set; }
	}
}
