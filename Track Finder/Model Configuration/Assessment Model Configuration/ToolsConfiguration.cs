using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackFinder.Models.AssessmentModels;

namespace TrackFinder.Infrastructure.Model_Validation.Assessment_Model_Configuration;

public class ToolsConfiguration : IEntityTypeConfiguration<Tools>
{
	public void Configure(EntityTypeBuilder<Tools> builder)
	{
		builder.ToTable("Tools");

		builder.HasKey(t => t.Id);

		builder.Property(t => t.Id)
			  .ValueGeneratedOnAdd();

		builder.Property(t => t.Name)
			  .IsRequired()
			  .HasMaxLength(150);

		builder.Property(t => t.Description)
			  .HasMaxLength(500);
	}
}
