using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackFinder.Models.CourseModels;

namespace TrackFinder.Infrastructure.Model_Validation.Course_Model_Configuration;

public class StudentAnswerConfiguration : IEntityTypeConfiguration<StudentAnswer>
{
	public void Configure(EntityTypeBuilder<StudentAnswer> builder)
	{
		builder.ToTable("StudentAnswers");

		builder.HasKey(sa => new { sa.AnswerId, sa.QuestionId, sa.ExamAttepmtId, sa.StudentId });

		builder.HasOne(sa => sa.ExamAttempt)
			  .WithMany(ea => ea.StudentAnswers)
			  .HasForeignKey(sa => sa.ExamAttepmtId)
			  .OnDelete(DeleteBehavior.Restrict);

		builder.HasOne(sa => sa.Question)
			  .WithMany(q => q.StudentAnswers)
			  .HasForeignKey(sa => sa.QuestionId)
			  .OnDelete(DeleteBehavior.Restrict);

		builder.HasOne(sa => sa.Answer)
			  .WithMany(o => o.StudentAnswers)
			  .HasForeignKey(sa => sa.AnswerId)
			  .OnDelete(DeleteBehavior.Restrict);

		builder.HasOne(sa => sa.Student)
			  .WithMany(s => s.StudentAnswers)
			  .HasForeignKey(sa => sa.StudentId)
			  .OnDelete(DeleteBehavior.Restrict);
	}
}
