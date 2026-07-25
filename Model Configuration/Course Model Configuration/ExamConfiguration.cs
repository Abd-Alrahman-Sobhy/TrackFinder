using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackFinder.Models.CourseModels;

namespace TrackFinder.Infrastructure.Model_Validation.Course_Model_Configuration;

public class ExamConfiguration : IEntityTypeConfiguration<Exam>
{
	public void Configure(EntityTypeBuilder<Exam> builder)
	{
		builder.ToTable("Exams");

		builder.HasKey(e => e.Id);

		builder.Property(e => e.Id)
			  .ValueGeneratedOnAdd();

		builder.Property(e => e.Description)
			  .HasMaxLength(1000);

		builder.HasOne(e => e.Lesson)
			  .WithMany(l => l.Exams)
			  .HasForeignKey(e => e.LessonId)
			  .OnDelete(DeleteBehavior.Cascade);
	}
}
