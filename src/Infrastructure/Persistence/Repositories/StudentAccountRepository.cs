using Interrapidisimo.Domain.Entities;
using Interrapidisimo.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Interrapidisimo.Infrastructure.Persistence.Repositories;

public class StudentAccountRepository(ApplicationDbContext context) : Repository<StudentAccount>(context), IStudentAccountRepository
{
    public async Task<StudentAccount?> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default) =>
        await Context.StudentAccounts
            .Include(a => a.Student)
            .FirstOrDefaultAsync(a => a.StudentId == studentId, cancellationToken);

    public async Task<StudentAccount?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        return await Context.StudentAccounts
            .Include(a => a.Student)
            .FirstOrDefaultAsync(a => a.Student != null && a.Student.Email == normalizedEmail, cancellationToken);
    }
}
