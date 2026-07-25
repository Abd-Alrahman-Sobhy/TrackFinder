using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackFinder.Models.AssessmentModels;

namespace TrackFinder.Infrastructure.Model_Validation.Assessment_Model_Configuration;

public class StackToolsConfiguration : IEntityTypeConfiguration<StackTools>
{
	public void Configure(EntityTypeBuilder<StackTools> builder)
	{
		builder.ToTable("StackTools");

		builder.HasKey(st => new { st.StackId, st.ToolId });

		builder.HasOne(st => st.RelatedStack)
			  .WithMany(s => s.RelatedStackTools)
			  .HasForeignKey(st => st.StackId)
			  .OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(st => st.RelatedTool)
			  .WithMany(t => t.RelatedStackTools)
			  .HasForeignKey(st => st.ToolId)
			  .OnDelete(DeleteBehavior.Cascade);
	}
}
