using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackFinder.Models.AssessmentModels;

namespace TrackFinder.Infrastructure.Model_Validation.Assessment_Model_Configuration;

public class AssessmentResultTracksConfiguration : IEntityTypeConfiguration<AssessmentResultTracks>
{
	public void Configure(EntityTypeBuilder<AssessmentResultTracks> builder)
	{
		builder.ToTable("AssessmentResultTracks");

		builder.HasKey(art => new { art.AssessmentResultId, art.TrackId });

		builder.HasOne(art => art.AssessmentResult)
			  .WithMany(ar => ar.RecommendedTracks)
			  .HasForeignKey(art => art.AssessmentResultId)
			  .OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(art => art.Track)
			  .WithMany(t => t.AssessmentResultTracks)
			  .HasForeignKey(art => art.TrackId)
			  .OnDelete(DeleteBehavior.Cascade);
	}
}
