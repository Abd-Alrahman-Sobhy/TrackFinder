using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackFinder.Models.AssessmentModels;

namespace TrackFinder.Infrastructure.Model_Validation.Assessment_Model_Configuration;

public class GainedSkillConfiguration : IEntityTypeConfiguration<GainedSkill>
{
	public void Configure(EntityTypeBuilder<GainedSkill> builder)
	{
		builder.ToTable("GainedSkills");

		builder.HasKey(gs => gs.Id);

		builder.Property(gs => gs.Id)
			  .ValueGeneratedOnAdd();

		builder.Property(gs => gs.Name)
			  .IsRequired()
			  .HasMaxLength(150);

		builder.Property(gs => gs.Description)
			  .HasMaxLength(500);
	}
}
