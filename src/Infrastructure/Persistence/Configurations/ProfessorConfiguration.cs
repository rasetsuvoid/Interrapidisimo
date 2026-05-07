using Interrapidisimo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Interrapidisimo.Infrastructure.Persistence.Configurations;

public class ProfessorConfiguration : IEntityTypeConfiguration<Professor>
{
    public void Configure(EntityTypeBuilder<Professor> builder)
    {
        builder.ToTable("Professors");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(e => e.LastName).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Email).IsRequired().HasMaxLength(255);
        builder.Property(e => e.Bio).HasMaxLength(500);
        builder.Property(e => e.CreatedBy).HasMaxLength(100);
        builder.Property(e => e.UpdatedBy).HasMaxLength(100);

        builder.HasIndex(e => e.Email).IsUnique().HasFilter("[IsDeleted] = 0");
        builder.HasIndex(e => e.IsDeleted);

        builder.HasMany(e => e.Subjects)
            .WithOne(e => e.Professor!)
            .HasForeignKey(e => e.ProfessorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
