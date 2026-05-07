namespace Interrapidisimo.Domain.Interfaces;

public interface IUnitOfWork
{
    IStudentRepository Students { get; }
    IStudentAccountRepository StudentAccounts { get; }
    ISubjectRepository Subjects { get; }
    IProfessorRepository Professors { get; }
    IEnrollmentRepository Enrollments { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
