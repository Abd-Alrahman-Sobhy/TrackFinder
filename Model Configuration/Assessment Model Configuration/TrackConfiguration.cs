using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackFinder.Models.AssessmentModels;
namespace TrackFinder.Infrastructure.Model_Validation.Assessment_Model_Configuration;
public class TrackConfiguration : IEntityTypeConfiguration<Track>
{
    public void Configure(EntityTypeBuilder<Track> builder)
    {
        builder.ToTable("Tracks");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
               .ValueGeneratedOnAdd();
        builder.Property(t => t.TrackName)
               .IsRequired()
               .HasMaxLength(150);
        builder.Property(t => t.TrackDescription)
               .IsRequired()
               .HasMaxLength(1000);
        builder.Property(t => t.RoadMapUrl)
               .HasMaxLength(500);
        builder.HasMany(t => t.QuestionWeights)
               .WithOne(q => q.RelatedTrack)
               .HasForeignKey(q => q.TrackId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}