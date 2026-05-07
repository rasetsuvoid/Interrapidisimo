using Interrapidisimo.Domain.Entities;

namespace Interrapidisimo.Domain.Interfaces;

public interface IEnrollmentRepository : IRepository<StudentSubject>
{
    Task<IEnumerable<StudentSubject>> GetByStudentAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<StudentSubject>> GetBySubjectAsync(Guid subjectId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Student>> GetClassmatesAsync(Guid studentId, Guid subjectId, CancellationToken cancellationToken = default);
    Task<bool> IsEnrolledAsync(Guid studentId, Guid subjectId, CancellationToken cancellationToken = default);
    void Delete(StudentSubject enrollment);
}
