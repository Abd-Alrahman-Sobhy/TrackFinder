using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackFinder.Models.AchievementModels;

namespace TrackFinder.Infrastructure.Model_Validation.Achievement_Model_Configuration;

public class BadgesConfiguration : IEntityTypeConfiguration<Badges>
{
	public void Configure(EntityTypeBuilder<Badges> builder)
	{
		builder.ToTable("Badges");

		builder.HasKey(b => b.BadgeId);

		builder.Property(b => b.BadgeId)
			  .ValueGeneratedOnAdd();

		builder.Property(b => b.BadgeName)
			  .HasMaxLength(100);

		builder.Property(b => b.BadgeDescription)
			  .HasMaxLength(500);

		builder.Property(b => b.BadgeImageUrl)
			  .HasMaxLength(500);
	}
}
