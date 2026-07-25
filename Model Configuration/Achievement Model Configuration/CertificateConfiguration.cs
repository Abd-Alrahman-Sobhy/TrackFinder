using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackFinder.Models.AchievementModels;

namespace TrackFinder.Infrastructure.Model_Validation.Achievement_Model_Configuration;

public class CertificateConfiguration : IEntityTypeConfiguration<Certificate>
{
	public void Configure(EntityTypeBuilder<Certificate> builder)
	{
		builder.ToTable("Certificates");

		builder.HasKey(c => c.Id);

		builder.Property(c => c.Id)
			  .ValueGeneratedOnAdd();

		builder.Property(c => c.Title)
			  .HasMaxLength(200);

		builder.Property(c => c.Description)
			  .HasMaxLength(1000);

		builder.Property(c => c.CertificateUrl)
			  .HasMaxLength(500);

		builder.Property(c => c.CertificateImageUrl)
			  .HasMaxLength(500);
	}
}
