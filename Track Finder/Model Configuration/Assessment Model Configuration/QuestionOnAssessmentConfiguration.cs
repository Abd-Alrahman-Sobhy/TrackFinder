using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackFinder.Models.AssessmentModels;

namespace TrackFinder.Data.Configurations
{
    public class QuestionOnAssessmentConfiguration
        : IEntityTypeConfiguration<QuestionOnAssessment>
    {
        public void Configure(EntityTypeBuilder<QuestionOnAssessment> builder)
        {
            builder.ToTable("QuestionsOnAssessments");

            builder.HasKey(q => q.Id);

            builder.Property(q => q.QuestionText)
                   .IsRequired()
                   .HasMaxLength(1000);

            builder.HasMany(q => q.QuestionWeights)
                   .WithOne(w => w.RelatedQuestion)
                   .HasForeignKey(w => w.QuestionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}