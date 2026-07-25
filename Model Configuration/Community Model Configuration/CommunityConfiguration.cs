using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackFinder.Models.CommunityModels;

namespace TrackFinder.Infrastructure.Model_Validation.Community_Model_Configuration;

public class CommunityConfiguration : IEntityTypeConfiguration<Community>
{
	public void Configure(EntityTypeBuilder<Community> builder)
	{
		builder.ToTable("Communities");

		builder.HasKey(c => c.Id);

		builder.Property(c => c.Id)
			  .ValueGeneratedOnAdd();

		builder.Property(c => c.Name)
			  .IsRequired()
			  .HasMaxLength(150);

		builder.HasOne(c => c.Admin)
			  .WithMany(i => i.AdminstratedCommunities)
			  .HasForeignKey(c => c.AdminId)
			  .OnDelete(DeleteBehavior.Restrict);

		builder.HasMany(c => c.Posts)
			  .WithOne(p => p.Community)
			  .HasForeignKey(p => p.GroupId)
			  .OnDelete(DeleteBehavior.Cascade);

		builder.HasMany(c => c.JoinedMembers)
			  .WithOne(jm => jm.Community)
			  .HasForeignKey(jm => jm.CommunityId)
			  .OnDelete(DeleteBehavior.Cascade);
	}
}
