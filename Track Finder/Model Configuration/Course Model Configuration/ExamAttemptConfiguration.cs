using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackFinder.Models.CourseModels;

namespace TrackFinder.Infrastructure.Model_Validation.Course_Model_Configuration;

public class ExamAttemptConfiguration : IEntityTypeConfiguration<ExamAttempt>
{
	public void Configure(EntityTypeBuilder<ExamAttempt> builder)
	{
		builder.ToTable("ExamAttempts");

		builder.HasKey(ea => ea.Id);

		builder.Property(ea => ea.Id)
			  .ValueGeneratedOnAdd();

		builder.Property(ea => ea.AttemptDate)
			  .HasDefaultValueSql("GETUTCDATE()");

		builder.HasOne(ea => ea.Exam)
			  .WithMany(e => e.ExamAttempts)
			  .HasForeignKey(ea => ea.ExamId)
			  .OnDelete(DeleteBehavior.Cascade);
	}
}
