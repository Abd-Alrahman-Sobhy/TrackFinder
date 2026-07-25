using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackFinder.Models.CommunityModels;

namespace TrackFinder.Infrastructure.Model_Validation.Community_Model_Configuration;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
	public void Configure(EntityTypeBuilder<Post> builder)
	{
		builder.ToTable("Posts");

		builder.HasKey(p => p.Id);

		builder.Property(p => p.Id)
			  .ValueGeneratedOnAdd();

		builder.Property(p => p.Content)
			  .IsRequired();

		builder.Property(p => p.ImageUrl)
			  .HasMaxLength(500);

		builder.Property(p => p.CreatedAt)
			  .HasDefaultValueSql("GETUTCDATE()");

		builder.Property(p => p.Status)
			  .HasConversion<int>();

		builder.HasOne(p => p.Author)
			  .WithMany(u => u.Posts)
			  .HasForeignKey(p => p.UserId)
			  .OnDelete(DeleteBehavior.Restrict);

		builder.HasMany(p => p.Comments)
			  .WithOne(c => c.Post)
			  .HasForeignKey(c => c.PostId)
			  .OnDelete(DeleteBehavior.Cascade);
	}
}
