using Interrapidisimo.Domain.Entities;

namespace Interrapidisimo.Domain.Interfaces;

public interface IProfessorRepository : IRepository<Professor>
{
    Task<IEnumerable<Professor>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Professor>> GetAllWithSubjectsAsync(CancellationToken cancellationToken = default);
}
