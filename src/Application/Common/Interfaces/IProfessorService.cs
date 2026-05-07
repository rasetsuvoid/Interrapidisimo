using Interrapidisimo.Application.Common.Models;
using Interrapidisimo.Application.Professors.DTOs;

namespace Interrapidisimo.Application.Common.Interfaces;

public interface IProfessorService
{
    Task<Result<ProfessorDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<ProfessorDto>>> GetAllAsync(CancellationToken cancellationToken = default);
}
