using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackFinder.Models.CommunityModels;

namespace TrackFinder.Infrastructure.Model_Validation.Community_Model_Configuration;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
	public void Configure(EntityTypeBuilder<Comment> builder)
	{
		builder.ToTable("Comments");

		builder.HasKey(c => c.Id);

		builder.Property(c => c.Id)
			  .ValueGeneratedOnAdd();

		builder.Property(c => c.Content)
			  .IsRequired();

		builder.Property(c => c.CreatedAt)
			  .HasDefaultValueSql("GETUTCDATE()");

		builder.HasOne(c => c.User)
			  .WithMany(u => u.Comments)
			  .HasForeignKey(c => c.UserId)
			  .OnDelete(DeleteBehavior.Restrict);

		builder.HasOne(c => c.ParentComment)
			  .WithMany(c => c.Replies)
			  .HasForeignKey(c => c.ParentCommentId)
			  .OnDelete(DeleteBehavior.Restrict);
	}
}
