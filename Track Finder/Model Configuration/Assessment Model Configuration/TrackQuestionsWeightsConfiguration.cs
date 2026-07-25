using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackFinder.Models.Assessment_Models;

namespace TrackFinder.Infrastructure.Model_Validation.Assessment_Model_Configuration
{
    public class TrackQuestionsWeightsConfiguration
        : IEntityTypeConfiguration<TrackQuestionsWeights>
    {
        public void Configure(EntityTypeBuilder<TrackQuestionsWeights> builder)
        {
            builder.ToTable("TrackQuestionsWeights");

            builder.HasKey(x => new
            {
                x.TrackId,
                x.QuestionId
            });

            builder.Property(x => x.Weight)
                   .HasColumnType("float")
                   .IsRequired();

            builder.HasOne(x => x.RelatedTrack)
                   .WithMany(t => t.QuestionWeights)
                   .HasForeignKey(x => x.TrackId);

            builder.HasOne(x => x.RelatedQuestion)
                   .WithMany(q => q.QuestionWeights)
                   .HasForeignKey(x => x.QuestionId);
        }
    }
}