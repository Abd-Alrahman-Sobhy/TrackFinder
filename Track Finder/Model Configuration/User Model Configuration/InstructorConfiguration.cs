using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackFinder.Models.UserModels;

namespace TrackFinder.Infrastructure.Model_Validation.User_Model_Configuration;

public class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
{
	public void Configure(EntityTypeBuilder<Instructor> builder)
	{
		builder.ToTable("Instructors");

		builder.HasKey(i => i.UserId);

		builder.Property(i => i.Title)
			  .HasMaxLength(100);

		builder.Property(i => i.GithubLink)
			  .HasMaxLength(300);

		builder.Property(i => i.LinkedInLink)
			  .HasMaxLength(300);

		builder.HasOne(i => i.User)
			  .WithOne(u => u.Instructor)
			  .HasForeignKey<Instructor>(i => i.UserId)
			  .OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(i => i.ModeratedCommunity)
			  .WithMany(c => c.Moderators)
			  .HasForeignKey(i => i.CommunityId)
			  .OnDelete(DeleteBehavior.SetNull);
	}
}
