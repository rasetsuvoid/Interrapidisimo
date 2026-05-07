using Interrapidisimo.Domain.Common;

namespace Interrapidisimo.Domain.Entities;

public class Subject : AuditableEntity
{
    public const int CreditsValue = 3;

    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public int Credits { get; private set; } = CreditsValue;
    public Guid ProfessorId { get; private set; }
    public Professor? Professor { get; private set; }

    private readonly List<StudentSubject> _enrollments = [];
    public IReadOnlyCollection<StudentSubject> Enrollments => _enrollments.AsReadOnly();

    public int EnrolledStudentsCount => _enrollments.Count(e => !e.IsDeleted);

    private Subject() { }

    public static Subject Create(
        string name,
        string code,
        Guid professorId,
        string? description = null,
        string? createdBy = null)
    {
        var subject = new Subject
        {
            Name = name.Trim(),
            Code = code.Trim().ToUpperInvariant(),
            ProfessorId = professorId,
            Description = description?.Trim(),
            Credits = CreditsValue
        };
        subject.SetCreated(createdBy);
        return subject;
    }
}
