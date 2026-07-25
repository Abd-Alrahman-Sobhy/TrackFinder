using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TrackFinder.Models.AchievementModels;
using TrackFinder.Models.AssessmentModels;
using TrackFinder.Models.CommunityModels;
using TrackFinder.Models.CourseModels;
using TrackFinder.Models.OrdersAndPaymentsModels;
using TrackFinder.Models.UserModels;
using TrackFinderDb.Models.TeachingModels;

namespace TrackFinder.Context;

public class AppDbContext : IdentityDbContext<User, Role, Guid>
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

	#region DbSets

	// ── User Module ──────────────────────────────────────────────
	public virtual DbSet<Student> Students { get; set; }
	public virtual DbSet<Instructor> Instructors { get; set; }

	// ── Achievement Module ───────────────────────────────────────
	public virtual DbSet<Badges> Badges { get; set; }
	public virtual DbSet<Certificate> Certificates { get; set; }
	public virtual DbSet<StudentCertificate> StudentCertificates { get; set; }
	public virtual DbSet<UserBadge> UserBadges { get; set; }

	// ── Assessment / Track Module ────────────────────────────────
	public virtual DbSet<Track> Tracks { get; set; }
	public virtual DbSet<TrackStack> TrackStacks { get; set; }
	public virtual DbSet<TrackSkills> TrackSkills { get; set; }
	public virtual DbSet<Tools> Tools { get; set; }
	public virtual DbSet<StackTools> StackTools { get; set; }
	public virtual DbSet<GainedSkill> GainedSkills { get; set; }
	public virtual DbSet<AssessmentResult> AssessmentResults { get; set; }
	public virtual DbSet<AssessmentResultTracks> AssessmentResultTracks { get; set; }
    public virtual DbSet<QuestionOnAssessment> QuestionsOnAssessment { get; set; }

    // ── Community Module ─────────────────────────────────────────
    public virtual DbSet<Community> Communities { get; set; }
	public virtual DbSet<Post> Posts { get; set; }
	public virtual DbSet<Comment> Comments { get; set; }
	public virtual DbSet<PostReport> PostReports { get; set; }
	public virtual DbSet<JoinedMembers> JoinedMembers { get; set; }

	// ── Teaching / Course Module ─────────────────────────────────
	public virtual DbSet<Course> Courses { get; set; }
	public virtual DbSet<CourseSkill> CourseSkills { get; set; }
	public virtual DbSet<Lesson> Lessons { get; set; }
	public virtual DbSet<Material> Materials { get; set; }
	public virtual DbSet<Enrollment> Enrollments { get; set; }

	// ── Exams & Quizzes Module ───────────────────────────────────
	public virtual DbSet<Exam> Exams { get; set; }
	public virtual DbSet<ExamAttempt> ExamAttempts { get; set; }
	public virtual DbSet<Question> Questions { get; set; }
	public virtual DbSet<Option> Options { get; set; }
	public virtual DbSet<StudentAnswer> StudentAnswers { get; set; }

	// ── Orders & Payments Module ─────────────────────────────────
	public virtual DbSet<Purchasing> PurchasedItems { get; set; }

	#endregion

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

		base.OnModelCreating(modelBuilder);
	}
}
