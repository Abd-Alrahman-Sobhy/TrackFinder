using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackFinder.Models.AssessmentModels;

namespace TrackFinder.Infrastructure.Model_Validation.Assessment_Model_Configuration;

public class TrackStackConfiguration : IEntityTypeConfiguration<TrackStack>
{
	public void Configure(EntityTypeBuilder<TrackStack> builder)
	{
		builder.ToTable("TrackStacks");

		builder.HasKey(ts => ts.Id);

		builder.Property(ts => ts.Id)
			  .ValueGeneratedOnAdd();

		builder.Property(ts => ts.StackName)
			  .IsRequired()
			  .HasMaxLength(150);

		builder.Property(ts => ts.StackDescription)
			  .HasMaxLength(500);

		builder.HasOne(ts => ts.RelatedTrack)
			  .WithMany(t => t.RelatedStacks)
			  .HasForeignKey(ts => ts.RelatedTrackId)
			  .OnDelete(DeleteBehavior.Cascade);

		builder.HasMany(ts => ts.Courses)
			  .WithOne(c => c.TrackStack)
			  .HasForeignKey(c => c.TrackStackId)
			  .OnDelete(DeleteBehavior.SetNull);
	}
}
