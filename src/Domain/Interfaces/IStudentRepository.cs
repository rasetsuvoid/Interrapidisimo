using Interrapidisimo.Domain.Entities;

namespace Interrapidisimo.Domain.Interfaces;

public interface IStudentRepository : IRepository<Student>
{
    Task<Student?> GetByIdWithEnrollmentsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Student?> GetByIdWithEnrollmentsNoTrackingAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Student?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Student?> GetByStudentCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IEnumerable<Student>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<(IEnumerable<Student> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? search = null, CancellationToken cancellationToken = default);
    void Delete(Student student);
    Task<bool> EmailExistsAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default);
}
