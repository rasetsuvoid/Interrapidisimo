using Interrapidisimo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Interrapidisimo.Infrastructure.Persistence.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Students");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(e => e.LastName).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Email).IsRequired().HasMaxLength(255);
        builder.Property(e => e.StudentCode).IsRequired().HasMaxLength(20);
        builder.Property(e => e.PhoneNumber).HasMaxLength(20);
        builder.Property(e => e.CreatedBy).HasMaxLength(100);
        builder.Property(e => e.UpdatedBy).HasMaxLength(100);

        builder.HasIndex(e => e.Email).IsUnique().HasFilter("[IsDeleted] = 0");
        builder.HasIndex(e => e.StudentCode).IsUnique().HasFilter("[IsDeleted] = 0");
        builder.HasIndex(e => e.IsDeleted);
        builder.HasIndex(e => e.CreatedAt);

        builder.HasMany(e => e.Enrollments)
            .WithOne(e => e.Student!)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
