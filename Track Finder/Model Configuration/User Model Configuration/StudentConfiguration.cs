using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackFinder.Models.UserModels;

namespace TrackFinder.Infrastructure.Model_Validation.User_Model_Configuration;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
	public void Configure(EntityTypeBuilder<Student> builder)
	{
		builder.ToTable("Students");

		builder.HasKey(s => s.UserId);

		builder.Property(s => s.EducationState)
			  .HasMaxLength(50);

		builder.Property(s => s.SchoolOrUnversityName)
			  .HasMaxLength(200);

		builder.Property(s => s.Major)
			  .HasMaxLength(100);

		builder.Property(s => s.Minor)
			  .HasMaxLength(100);

		builder.Property(s => s.DegreeProgram)
			  .HasMaxLength(100);

		builder.Property(s => s.Bio)
			  .HasMaxLength(500);

		builder.HasOne(s => s.User)
			  .WithOne(u => u.Student)
			  .HasForeignKey<Student>(s => s.UserId)
			  .OnDelete(DeleteBehavior.Cascade);
	}
}
