using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackFinder.Models.CourseModels;
namespace TrackFinder.Infrastructure.Model_Validation.Course_Model_Configuration;
public class CourseSkillConfiguration : IEntityTypeConfiguration<CourseSkill>
{
    public void Configure(EntityTypeBuilder<CourseSkill> builder)
    {
        builder.ToTable("CourseSkills");
        builder.HasKey(cs => new { cs.CourseId, cs.GainedSkillId });
        builder.HasOne(cs => cs.Course)
              .WithMany(c => c.CourseSkills)
              .HasForeignKey(cs => cs.CourseId)
              .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(cs => cs.GainedSkill)
              .WithMany(gs => gs.Courses)
              .HasForeignKey(cs => cs.GainedSkillId)
              .OnDelete(DeleteBehavior.Cascade);
    }
}