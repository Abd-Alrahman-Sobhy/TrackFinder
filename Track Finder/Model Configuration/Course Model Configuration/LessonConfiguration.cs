using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackFinder.Models.CourseModels;

namespace TrackFinder.Infrastructure.Model_Validation.Course_Model_Configuration;

public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{
	public void Configure(EntityTypeBuilder<Lesson> builder)
	{
		builder.ToTable("Lessons");

		builder.HasKey(l => l.Id);

		builder.Property(l => l.Id)
			  .ValueGeneratedOnAdd();

		builder.Property(l => l.Name)
			  .IsRequired()
			  .HasMaxLength(200);

		builder.Property(l => l.Description)
			  .HasMaxLength(1000);

		builder.OwnsOne(l => l.Duration, d =>
		{
			d.Property(x => x.Minutes)
			 .HasColumnName("DurationMinutes");

			d.Property(x => x.Seconds)
			 .HasColumnName("DurationSeconds");
		});

		builder.HasOne(l => l.Course)
			  .WithMany(c => c.Lessons)
			  .HasForeignKey(l => l.CourseId)
			  .OnDelete(DeleteBehavior.SetNull);
	}
}
