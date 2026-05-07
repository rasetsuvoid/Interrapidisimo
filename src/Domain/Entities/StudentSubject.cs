using Interrapidisimo.Domain.Common;

namespace Interrapidisimo.Domain.Entities;

public class StudentSubject : AuditableEntity
{
    public Guid StudentId { get; private set; }
    public Student? Student { get; private set; }
    public Guid SubjectId { get; private set; }
    public Subject? Subject { get; private set; }
    public DateTime EnrolledAt { get; private set; }

    private StudentSubject() { }

    public static StudentSubject Create(Guid studentId, Guid subjectId, Subject? subject = null, string? createdBy = null)
    {
        var enrollment = new StudentSubject
        {
            StudentId = studentId,
            SubjectId = subjectId,
            Subject = subject,
            EnrolledAt = DateTime.UtcNow
        };
        enrollment.SetCreated(createdBy);
        return enrollment;
    }
}
