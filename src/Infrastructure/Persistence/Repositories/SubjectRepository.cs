using Interrapidisimo.Domain.Entities;
using Interrapidisimo.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Interrapidisimo.Infrastructure.Persistence.Repositories;

public class SubjectRepository(ApplicationDbContext context) : Repository<Subject>(context), ISubjectRepository
{
    public async Task<Subject?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default) =>
        await Context.Subjects
            .Include(s => s.Professor)
            .Include(s => s.Enrollments.Where(e => !e.IsDeleted))
                .ThenInclude(e => e.Student)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<Subject?> GetByIdWithProfessorAsync(Guid id, CancellationToken cancellationToken = default) =>
        await Context.Subjects
            .AsNoTracking()
            .Include(s => s.Professor)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<IEnumerable<Subject>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await Context.Subjects.OrderBy(s => s.Code).ToListAsync(cancellationToken);

    public async Task<IEnumerable<Subject>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default) =>
        await Context.Subjects
            .Include(s => s.Professor)
            .Include(s => s.Enrollments.Where(e => !e.IsDeleted))
            .OrderBy(s => s.Code)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Subject>> GetByProfessorAsync(Guid professorId, CancellationToken cancellationToken = default) =>
        await Context.Subjects
            .Where(s => s.ProfessorId == professorId)
            .OrderBy(s => s.Code)
            .ToListAsync(cancellationToken);
}
