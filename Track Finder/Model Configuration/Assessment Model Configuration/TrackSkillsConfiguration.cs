using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackFinder.Models.AssessmentModels;
namespace TrackFinder.Infrastructure.Model_Validation.Assessment_Model_Configuration;
public class TrackSkillsConfiguration : IEntityTypeConfiguration<TrackSkills>
{
    public void Configure(EntityTypeBuilder<TrackSkills> builder)
    {
        builder.ToTable("TrackSkills");
        builder.HasKey(tsk => new { tsk.TrackId, tsk.SkillsId });

        builder.HasOne(tsk => tsk.RelatedTrack)
              .WithMany(t => t.GainedSkills)
              .HasForeignKey(tsk => tsk.TrackId)
              .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(tsk => tsk.GainedSkill)
              .WithMany(gs => gs.TrackSkills)
              .HasForeignKey(tsk => tsk.SkillsId)
              .OnDelete(DeleteBehavior.Cascade);
    }
}