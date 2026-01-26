using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineLearningPlatformAss2.Data.Database.Entities;

namespace OnlineLearningPlatformAss2.Data.Database.EntityConfigurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
  public void Configure(EntityTypeBuilder<Role> builder)
  {
    // Primary Key
    builder.HasKey(r => r.Id);

    // Properties
    builder.Property(r => r.Name)
        .IsRequired()
        .HasMaxLength(100);

    builder.Property(r => r.Description)
        .IsRequired()
        .HasMaxLength(500);

    builder.Property(r => r.CreatedAt)
        .HasDefaultValue(DateTime.UtcNow);

    builder.Property(r => r.IsDeleted)
        .HasDefaultValue(false);

    // Relationships
    builder.HasMany(r => r.Users)
        .WithOne(u => u.Role)
        .HasForeignKey(u => u.RoleId)
        .OnDelete(DeleteBehavior.SetNull);

    // Indices
    builder.HasIndex(r => r.Name)
        .IsUnique()
        .HasFilter("[IsDeleted] = 0");

    // Seed Data
    builder.HasData(
        new Role
        {
          Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
          Name = "Admin",
          Description = "Administrator with full access",
          CreatedAt = DateTime.UtcNow,
          IsDeleted = false
        },
        new Role
        {
          Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
          Name = "Instructor",
          Description = "Course instructor",
          CreatedAt = DateTime.UtcNow,
          IsDeleted = false
        },
        new Role
        {
          Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
          Name = "Student",
          Description = "Student learner",
          CreatedAt = DateTime.UtcNow,
          IsDeleted = false
        }
    );
  }
}