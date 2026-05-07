using Interrapidisimo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Interrapidisimo.Infrastructure.Persistence.Configurations;

public class StudentSubjectConfiguration : IEntityTypeConfiguration<StudentSubject>
{
    public void Configure(EntityTypeBuilder<StudentSubject> builder)
    {
        builder.ToTable("StudentSubjects");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.EnrolledAt).IsRequired();
        builder.Property(e => e.CreatedBy).HasMaxLength(100);
        builder.Property(e => e.UpdatedBy).HasMaxLength(100);

        builder.HasIndex(e => new { e.StudentId, e.SubjectId }).IsUnique().HasFilter("[IsDeleted] = 0");
        builder.HasIndex(e => e.StudentId);
        builder.HasIndex(e => e.SubjectId);
        builder.HasIndex(e => e.IsDeleted);
    }
}
