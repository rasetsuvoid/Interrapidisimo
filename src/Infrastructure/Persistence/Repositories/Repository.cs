using Interrapidisimo.Domain.Common;
using Interrapidisimo.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Interrapidisimo.Infrastructure.Persistence.Repositories;

public abstract class Repository<T>(ApplicationDbContext context) : IRepository<T>
    where T : BaseEntity
{
    protected readonly ApplicationDbContext Context = context;

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await Context.Set<T>().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default) =>
        await Context.Set<T>().AddAsync(entity, cancellationToken);

    public void Update(T entity) => Context.Set<T>().Update(entity);

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) =>
        await Context.Set<T>().AnyAsync(e => e.Id == id, cancellationToken);
}
