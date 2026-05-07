using Interrapidisimo.Application.Common.Models;
using Interrapidisimo.Application.Subjects.DTOs;

namespace Interrapidisimo.Application.Common.Interfaces;

public interface ISubjectService
{
    Task<Result<SubjectDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<SubjectDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<SubjectDto>>> GetByProfessorAsync(Guid professorId, CancellationToken cancellationToken = default);
}
