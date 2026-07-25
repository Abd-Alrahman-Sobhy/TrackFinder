using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackFinder.Models.UserModels;

namespace TrackFinder.Infrastructure.Model_Validation.User_Model_Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.FirstName)
              .IsRequired()
              .HasMaxLength(100);

        builder.Property(u => u.LastName)
              .HasMaxLength(100);

        builder.Property(u => u.Gender)
              .HasConversion<string>()
              .HasMaxLength(20);

        builder.Property(u => u.Bio)
              .HasMaxLength(500);

        builder.Property(u => u.ProfilePictureUrl)
              .HasMaxLength(500);

        builder.Property(u => u.Role)
              .IsRequired()
              .HasConversion<string>()
              .HasMaxLength(20);

        builder.Property(u => u.CreatedAt)
              .HasDefaultValueSql("GETUTCDATE()");
    }
}