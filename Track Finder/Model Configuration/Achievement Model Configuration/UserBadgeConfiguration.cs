using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackFinder.Models.AchievementModels;

namespace TrackFinder.Infrastructure.Model_Validation.Achievement_Model_Configuration;

internal class UserBadgeConfiguration : IEntityTypeConfiguration<UserBadge>
{
	public void Configure(EntityTypeBuilder<UserBadge> builder)
	{
		builder.ToTable("UserBadges");

		builder.HasKey(ub => new { ub.UserId, ub.BadgeId });

		builder.HasOne(ub => ub.User)
			  .WithMany(u => u.UserBadges)
			  .HasForeignKey(ub => ub.UserId)
			  .OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(ub => ub.Badge)
			  .WithMany(b => b.UserBadges)
			  .HasForeignKey(ub => ub.BadgeId)
			  .OnDelete(DeleteBehavior.Cascade);
	}
}
