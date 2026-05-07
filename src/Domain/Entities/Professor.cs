using Interrapidisimo.Domain.Common;

namespace Interrapidisimo.Domain.Entities;

public class Professor : AuditableEntity
{
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? Bio { get; private set; }

    private readonly List<Subject> _subjects = [];
    public IReadOnlyCollection<Subject> Subjects => _subjects.AsReadOnly();

    public string FullName => $"{FirstName} {LastName}";

    private Professor() { }

    public static Professor Create(
        string firstName,
        string lastName,
        string email,
        string? bio = null,
        string? createdBy = null)
    {
        var professor = new Professor
        {
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            Bio = bio?.Trim()
        };
        professor.SetCreated(createdBy);
        return professor;
    }
}
