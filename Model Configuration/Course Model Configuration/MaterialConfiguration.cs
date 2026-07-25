using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackFinder.Models.CourseModels;

namespace TrackFinder.Infrastructure.Model_Validation.Course_Model_Configuration;

public class MaterialConfiguration : IEntityTypeConfiguration<Material>
{
	public void Configure(EntityTypeBuilder<Material> builder)
	{
		builder.ToTable("Materials");

		builder.HasKey(m => m.Id);

		builder.Property(m => m.Id)
			  .ValueGeneratedOnAdd();

		builder.Property(m => m.FileName)
			  .IsRequired()
			  .HasMaxLength(200);

		builder.Property(m => m.FileUrl)
			  .IsRequired()
			  .HasMaxLength(500);

		builder.Property(m => m.ContentType)
			  .IsRequired()
			  .HasMaxLength(100);

		builder.HasOne(m => m.Lesson)
			  .WithMany(l => l.Materials)
			  .HasForeignKey(m => m.LessonId)
			  .OnDelete(DeleteBehavior.Restrict);

		builder.HasOne(m => m.Course)
			  .WithMany(c => c.Materials)
			  .HasForeignKey(m => m.CourseId)
			  .OnDelete(DeleteBehavior.Cascade);
	}
}
