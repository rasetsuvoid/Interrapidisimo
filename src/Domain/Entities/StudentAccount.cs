using Interrapidisimo.Domain.Common;

namespace Interrapidisimo.Domain.Entities;

public class StudentAccount : AuditableEntity
{
    public Guid StudentId { get; private set; }
    public Student? Student { get; private set; }
    public string PasswordHash { get; private set; } = string.Empty;

    private StudentAccount() { }

    public static StudentAccount Create(Guid studentId, string passwordHash, string? createdBy = null)
    {
        var account = new StudentAccount
        {
            StudentId = studentId,
            PasswordHash = passwordHash
        };

        account.SetCreated(createdBy);
        return account;
    }

    public void UpdatePassword(string passwordHash, string? updatedBy = null)
    {
        PasswordHash = passwordHash;
        SetUpdated(updatedBy);
    }
}
