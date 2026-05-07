using Interrapidisimo.Domain.Entities;
using Interrapidisimo.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Interrapidisimo.Infrastructure.Persistence.Repositories;

public class EnrollmentRepository(ApplicationDbContext context) : Repository<StudentSubject>(context), IEnrollmentRepository
{
    public override async Task<StudentSubject?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await Context.StudentSubjects
            .Include(e => e.Student)
            .Include(e => e.Subject)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<IEnumerable<StudentSubject>> GetByStudentAsync(Guid studentId, CancellationToken cancellationToken = default) =>
        await Context.StudentSubjects
            .Include(e => e.Subject).ThenInclude(s => s!.Professor)
            .Where(e => e.StudentId == studentId)
            .OrderBy(e => e.EnrolledAt)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<StudentSubject>> GetBySubjectAsync(Guid subjectId, CancellationToken cancellationToken = default) =>
        await Context.StudentSubjects
            .Include(e => e.Student)
            .Where(e => e.SubjectId == subjectId)
            .OrderBy(e => e.EnrolledAt)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Student>> GetClassmatesAsync(
        Guid studentId, Guid subjectId, CancellationToken cancellationToken = default) =>
        await Context.StudentSubjects
            .Include(e => e.Student)
            .Where(e => e.SubjectId == subjectId && e.StudentId != studentId && !e.IsDeleted)
            .Select(e => e.Student!)
            .Where(s => s != null && !s.IsDeleted)
            .OrderBy(s => s.LastName)
            .ToListAsync(cancellationToken);

    public async Task<bool> IsEnrolledAsync(Guid studentId, Guid subjectId, CancellationToken cancellationToken = default) =>
        await Context.StudentSubjects
            .AnyAsync(e => e.StudentId == studentId && e.SubjectId == subjectId, cancellationToken);

    public void Delete(StudentSubject enrollment) => Context.StudentSubjects.Update(enrollment);
}
