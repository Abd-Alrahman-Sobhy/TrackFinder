using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackFinder.Models.CommunityModels;

namespace TrackFinder.Infrastructure.Model_Validation.Community_Model_Configuration;

public class PostReportConfiguration : IEntityTypeConfiguration<PostReport>
{
	public void Configure(EntityTypeBuilder<PostReport> builder)
	{
		builder.ToTable("PostReports");

		builder.HasKey(pr => pr.Id);

		builder.Property(pr => pr.Id)
			  .ValueGeneratedOnAdd();

		builder.Property(pr => pr.Reason)
			  .HasConversion<int>();

		builder.Property(pr => pr.Description)
			  .HasMaxLength(1000);

		builder.Property(pr => pr.CreatedAt)
			  .HasDefaultValueSql("GETUTCDATE()");

		builder.HasOne(pr => pr.Post)
			  .WithMany()
			  .HasForeignKey(pr => pr.PostId)
			  .OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(pr => pr.Reporter)
			  .WithMany(u => u.PostReports)
			  .HasForeignKey(pr => pr.ReporterId)
			  .OnDelete(DeleteBehavior.Restrict);
	}
}
