using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackFinder.Models.CourseModels;

namespace TrackFinder.Infrastructure.Model_Validation.Course_Model_Validation;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
	public void Configure(EntityTypeBuilder<Course> builder)
	{
		builder.ToTable("Courses");

		builder.HasKey(c => c.Id);

		builder.Property(c => c.Id)
			  .ValueGeneratedOnAdd();

		builder.Property(c => c.Name)
			  .IsRequired()
			  .HasMaxLength(200);

		builder.Property(c => c.Description)
			  .HasMaxLength(2000);

		builder.Property(c => c.ImageUrl)
			  .HasMaxLength(500);

		builder.Property(c => c.Language)
			  .HasConversion<int>();

		builder.Property(c => c.Level)
			  .HasConversion<int>();

		builder.OwnsOne(c => c.Duration, d =>
		{
			d.Property(x => x.Value)
			 .HasColumnName("DurationValue");

			d.Property(x => x.DurationIn)
			 .HasColumnName("DurationUnit")
			 .HasConversion<int>();
		});

		builder.HasOne(c => c.Instructor)
			  .WithMany(i => i.CreatedCourses)
			  .HasForeignKey(c => c.InstructorId)
			  .OnDelete(DeleteBehavior.Restrict);
	}
}
