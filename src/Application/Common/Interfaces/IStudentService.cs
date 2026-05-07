using Interrapidisimo.Application.Common.Models;
using Interrapidisimo.Application.Students.DTOs;

namespace Interrapidisimo.Application.Common.Interfaces;

public interface IStudentService
{
    Task<Result<StudentDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<StudentDto>>> GetPagedAsync(int page, int pageSize, string? search = null, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<StudentDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<StudentDto>> CreateAsync(CreateStudentDto dto, CancellationToken cancellationToken = default);
    Task<Result<StudentDto>> UpdateAsync(Guid id, UpdateStudentDto dto, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
