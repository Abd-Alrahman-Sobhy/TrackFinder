using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackFinder.Models.CommunityModels;

namespace TrackFinder.Infrastructure.Model_Validation.Community_Model_Configuration;

public class JoinedMembersConfiguration : IEntityTypeConfiguration<JoinedMembers>
{
	public void Configure(EntityTypeBuilder<JoinedMembers> builder)
	{
		builder.ToTable("JoinedMembers");

		builder.HasKey(jm => new { jm.MemberId, jm.CommunityId });

		builder.HasOne(jm => jm.Member)
			  .WithMany(s => s.JoinedCommunities)
			  .HasForeignKey(jm => jm.MemberId)
			  .OnDelete(DeleteBehavior.Cascade);
	}
}
