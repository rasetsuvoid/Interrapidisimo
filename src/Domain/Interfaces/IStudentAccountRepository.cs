using Interrapidisimo.Domain.Entities;

namespace Interrapidisimo.Domain.Interfaces;

public interface IStudentAccountRepository : IRepository<StudentAccount>
{
    Task<StudentAccount?> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<StudentAccount?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
