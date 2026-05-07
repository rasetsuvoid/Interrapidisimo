using Interrapidisimo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Interrapidisimo.Infrastructure.Persistence.Configurations;

public class StudentAccountConfiguration : IEntityTypeConfiguration<StudentAccount>
{
    public void Configure(EntityTypeBuilder<StudentAccount> builder)
    {
        builder.ToTable("StudentAccounts");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.PasswordHash).IsRequired().HasMaxLength(512);
        builder.Property(e => e.CreatedBy).HasMaxLength(100);
        builder.Property(e => e.UpdatedBy).HasMaxLength(100);

        builder.HasIndex(e => e.StudentId).IsUnique().HasFilter("[IsDeleted] = 0");
        builder.HasIndex(e => e.IsDeleted);

        builder.HasOne(e => e.Student)
            .WithOne()
            .HasForeignKey<StudentAccount>(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
