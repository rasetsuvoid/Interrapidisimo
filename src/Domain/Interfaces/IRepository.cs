using Interrapidisimo.Domain.Common;

namespace Interrapidisimo.Domain.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
