using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackFinder.Models.AchievementModels;

namespace TrackFinder.Infrastructure.Model_Validation.Achievement_Model_Configuration;

public class StudentCertificateConfiguration : IEntityTypeConfiguration<StudentCertificate>
{
	public void Configure(EntityTypeBuilder<StudentCertificate> builder)
	{
		builder.ToTable("StudentCertificates");

		builder.HasKey(sc => sc.CredentialsId);
		builder.Property(sc => sc.CredentialsId)
			  .HasMaxLength(50);

		builder.HasOne(sc => sc.Certificate)
			  .WithMany(c => c.UserCertificates)
			  .HasForeignKey(sc => sc.CertificateId)
			  .OnDelete(DeleteBehavior.Restrict);

		builder.HasOne(sc => sc.Course)
			  .WithMany()
			  .HasForeignKey(sc => sc.CourseId)
			  .OnDelete(DeleteBehavior.Restrict);

		builder.HasOne(sc => sc.Student)
			  .WithMany()
			  .HasForeignKey(sc => sc.StudentId)
			  .OnDelete(DeleteBehavior.Cascade);
	}
}
