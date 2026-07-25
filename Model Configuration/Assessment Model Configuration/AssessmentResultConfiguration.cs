using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackFinder.Models.AssessmentModels;

namespace TrackFinder.Infrastructure.Model_Validation.Assessment_Model_Configuration;

public class AssessmentResultConfiguration : IEntityTypeConfiguration<AssessmentResult>
{
	public void Configure(EntityTypeBuilder<AssessmentResult> builder)
	{
		builder.ToTable("AssessmentResults");

		builder.HasKey(ar => ar.AssessmentResultId);

		builder.Property(ar => ar.AssessmentResultId)
			  .ValueGeneratedOnAdd();

		builder.Property(ar => ar.CreatedAt)
			  .HasDefaultValueSql("GETUTCDATE()");

		builder.HasOne(ar => ar.Student)
			  .WithMany(s => s.AssessmentResults)
			  .HasForeignKey(ar => ar.UserId)
			  .OnDelete(DeleteBehavior.Cascade);
	}
}
