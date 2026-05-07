using Interrapidisimo.Domain.Entities;
using Interrapidisimo.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Interrapidisimo.Infrastructure.Persistence.Repositories;

public class StudentRepository(ApplicationDbContext context) : Repository<Student>(context), IStudentRepository
{
    public async Task<Student?> GetByIdWithEnrollmentsAsync(Guid id, CancellationToken cancellationToken = default) =>
        await Context.Students
            .Include(s => s.Enrollments.Where(e => !e.IsDeleted))
                .ThenInclude(e => e.Subject)
                    .ThenInclude(sub => sub!.Professor)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<Student?> GetByIdWithEnrollmentsNoTrackingAsync(Guid id, CancellationToken cancellationToken = default) =>
        await Context.Students
            .AsNoTracking()
            .Include(s => s.Enrollments.Where(e => !e.IsDeleted))
                .ThenInclude(e => e.Subject)
                    .ThenInclude(sub => sub!.Professor)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<Student?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        await Context.Students.FirstOrDefaultAsync(s => s.Email == email.ToLowerInvariant(), cancellationToken);

    public async Task<Student?> GetByStudentCodeAsync(string code, CancellationToken cancellationToken = default) =>
        await Context.Students.FirstOrDefaultAsync(s => s.StudentCode == code.ToUpperInvariant(), cancellationToken);

    public async Task<IEnumerable<Student>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await Context.Students
            .Include(s => s.Enrollments.Where(e => !e.IsDeleted))
            .OrderBy(s => s.LastName).ThenBy(s => s.FirstName)
            .ToListAsync(cancellationToken);

    public async Task<(IEnumerable<Student> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? search = null, CancellationToken cancellationToken = default)
    {
        var query = Context.Students
            .Include(s => s.Enrollments.Where(e => !e.IsDeleted))
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(s =>
                s.FirstName.ToLower().Contains(term) ||
                s.LastName.ToLower().Contains(term) ||
                s.Email.ToLower().Contains(term) ||
                s.StudentCode.ToLower().Contains(term));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(s => s.LastName).ThenBy(s => s.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public void Delete(Student student) => Context.Students.Update(student);

    public async Task<bool> EmailExistsAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default) =>
        await Context.Students.AnyAsync(s =>
            s.Email == email.ToLowerInvariant() && (excludeId == null || s.Id != excludeId), cancellationToken);

    public async Task<bool> CodeExistsAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default) =>
        await Context.Students.AnyAsync(s =>
            s.StudentCode == code.ToUpperInvariant() && (excludeId == null || s.Id != excludeId), cancellationToken);
}
