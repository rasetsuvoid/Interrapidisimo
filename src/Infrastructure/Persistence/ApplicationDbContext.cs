using Interrapidisimo.Domain.Common;
using Interrapidisimo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Interrapidisimo.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Student> Students => Set<Student>();
    public DbSet<StudentAccount> StudentAccounts => Set<StudentAccount>();
    public DbSet<Professor> Professors => Set<Professor>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<StudentSubject> StudentSubjects => Set<StudentSubject>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Filtros globales para borrado lógico (soft delete)
        modelBuilder.Entity<Student>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<StudentAccount>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Professor>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Subject>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<StudentSubject>().HasQueryFilter(e => !e.IsDeleted);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries<AuditableEntity>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added && entry.Entity.CreatedAt == default)
                entry.Entity.SetCreated();

            if (entry.State == EntityState.Modified)
                entry.Entity.SetUpdated();
        }
    }
}
