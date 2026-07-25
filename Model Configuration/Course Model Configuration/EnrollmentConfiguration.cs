using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackFinder.Models.CourseModels;
using TrackFinderDb.Models.TeachingModels;

namespace TrackFinder.Infrastructure.Model_Validation.Course_Model_Configuration;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
	public void Configure(EntityTypeBuilder<Enrollment> builder)
	{
		builder.ToTable("Enrollments");

		builder.HasKey(e => new { e.CourseId, e.UserId });

		builder.Property(e => e.Status)
			  .HasConversion<int>();

		builder.Property(e => e.EnrollmentDate)
			  .HasDefaultValueSql("GETUTCDATE()");

		builder.HasOne(e => e.Course)
			  .WithMany(c => c.Enrollments)
			  .HasForeignKey(e => e.CourseId)
			  .OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(e => e.User)
			  .WithMany()
			  .HasForeignKey(e => e.UserId)
			  .OnDelete(DeleteBehavior.Cascade);
	}
}
