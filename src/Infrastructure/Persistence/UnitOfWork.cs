using Interrapidisimo.Domain.Interfaces;
using Interrapidisimo.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Interrapidisimo.Infrastructure.Persistence;

public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork, IDisposable
{
    private IDbContextTransaction? _transaction;
    private bool _disposed;

    public IStudentRepository Students { get; } = new StudentRepository(context);
    public IStudentAccountRepository StudentAccounts { get; } = new StudentAccountRepository(context);
    public ISubjectRepository Subjects { get; } = new SubjectRepository(context);
    public IProfessorRepository Professors { get; } = new ProfessorRepository(context);
    public IEnrollmentRepository Enrollments { get; } = new EnrollmentRepository(context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await context.SaveChangesAsync(cancellationToken);

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default) =>
        _transaction = await context.Database.BeginTransactionAsync(cancellationToken);

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
            await _transaction.CommitAsync(cancellationToken);
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
            await _transaction.RollbackAsync(cancellationToken);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _transaction?.Dispose();
            context.Dispose();
        }
        _disposed = true;
    }
}
