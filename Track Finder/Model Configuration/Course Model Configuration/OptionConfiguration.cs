using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackFinder.Models.CourseModels;

namespace TrackFinder.Infrastructure.Model_Validation.Course_Model_Configuration;

internal class OptionConfiguration : IEntityTypeConfiguration<Option>
{
	public void Configure(EntityTypeBuilder<Option> builder)
	{
		builder.ToTable("Options");

		builder.HasKey(o => o.Id);

		builder.Property(o => o.Id)
			  .ValueGeneratedOnAdd();

		builder.Property(o => o.OptionText)
			  .IsRequired()
			  .HasMaxLength(500);

		builder.HasOne(o => o.Question)
			  .WithMany(q => q.Options)
			  .HasForeignKey(o => o.QuestionId)
			  .OnDelete(DeleteBehavior.Cascade);
	}
}
