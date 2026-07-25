using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackFinder.Models.CourseModels;

namespace TrackFinder.Infrastructure.Model_Validation.Course_Model_Configuration;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
	public void Configure(EntityTypeBuilder<Question> builder)
	{
		builder.ToTable("Questions");

		builder.HasKey(q => q.Id);

		builder.Property(q => q.Id)
			  .ValueGeneratedOnAdd();

		builder.Property(q => q.QuestionText)
			  .IsRequired();

		builder.Property(q => q.QuestionType)
			  .HasConversion<int>();

		builder.HasOne(q => q.Exam)
			  .WithMany(e => e.Questions)
			  .HasForeignKey(q => q.ExamId)
			  .OnDelete(DeleteBehavior.Cascade);
	}
}
