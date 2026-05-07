using Interrapidisimo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Interrapidisimo.Infrastructure.Persistence.Configurations;

public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.ToTable("Subjects");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Code).IsRequired().HasMaxLength(20);
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.Property(e => e.Credits).IsRequired();
        builder.Property(e => e.CreatedBy).HasMaxLength(100);
        builder.Property(e => e.UpdatedBy).HasMaxLength(100);

        builder.HasIndex(e => e.Code).IsUnique().HasFilter("[IsDeleted] = 0");
        builder.HasIndex(e => e.ProfessorId);
        builder.HasIndex(e => e.IsDeleted);

        builder.HasMany(e => e.Enrollments)
            .WithOne(e => e.Subject!)
            .HasForeignKey(e => e.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
