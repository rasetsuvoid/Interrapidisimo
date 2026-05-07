using Interrapidisimo.Domain.Entities;
using Interrapidisimo.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Interrapidisimo.Infrastructure.Persistence.Repositories;

public class ProfessorRepository(ApplicationDbContext context) : Repository<Professor>(context), IProfessorRepository
{
    public override async Task<Professor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await Context.Professors
            .Include(p => p.Subjects.Where(s => !s.IsDeleted))
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<IEnumerable<Professor>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await Context.Professors.OrderBy(p => p.LastName).ToListAsync(cancellationToken);

    public async Task<IEnumerable<Professor>> GetAllWithSubjectsAsync(CancellationToken cancellationToken = default) =>
        await Context.Professors
            .Include(p => p.Subjects.Where(s => !s.IsDeleted))
            .OrderBy(p => p.LastName)
            .ToListAsync(cancellationToken);
}
