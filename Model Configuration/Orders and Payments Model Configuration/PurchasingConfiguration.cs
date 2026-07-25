using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackFinder.Models.OrdersAndPaymentsModels;

namespace TrackFinder.Infrastructure.Model_Validation.Orders_and_Payments_Model_Configuration;

public class PurchasingConfiguration : IEntityTypeConfiguration<Purchasing>
{
	public void Configure(EntityTypeBuilder<Purchasing> builder)
	{
		builder.ToTable("PurchasedItems");

		builder.HasKey(pi => pi.PurchasingId);

		builder.Property(pi => pi.PurchasingId)
			  .ValueGeneratedOnAdd();

		builder.Property(pi => pi.type)
			  .HasColumnName("Type")
			  .IsRequired()
			  .HasMaxLength(50);

		builder.Property(pi => pi.PurchaseDate)
			  .HasDefaultValueSql("GETUTCDATE()");

		builder.HasOne(pi => pi.Student)
			  .WithMany()
			  .HasForeignKey(pi => pi.StudentId)
			  .OnDelete(DeleteBehavior.Restrict);

		builder.HasOne(pi => pi.Instructor)
			  .WithMany()
			  .HasForeignKey(pi => pi.InstructorId)
			  .OnDelete(DeleteBehavior.Restrict);
	}
}
