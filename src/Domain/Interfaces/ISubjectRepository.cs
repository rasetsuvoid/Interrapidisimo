using Interrapidisimo.Domain.Entities;

namespace Interrapidisimo.Domain.Interfaces;

public interface ISubjectRepository : IRepository<Subject>
{
    Task<Subject?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Subject?> GetByIdWithProfessorAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Subject>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Subject>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Subject>> GetByProfessorAsync(Guid professorId, CancellationToken cancellationToken = default);
}
