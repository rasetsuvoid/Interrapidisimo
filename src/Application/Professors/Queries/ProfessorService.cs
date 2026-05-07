using Interrapidisimo.Application.Common.Interfaces;
using Interrapidisimo.Application.Common.Models;
using Interrapidisimo.Application.Professors.DTOs;
using Interrapidisimo.Application.Subjects.DTOs;
using Interrapidisimo.Domain.Interfaces;

namespace Interrapidisimo.Application.Professors.Queries;

public class ProfessorService(IUnitOfWork unitOfWork) : IProfessorService
{
    public async Task<Result<ProfessorDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var professor = await unitOfWork.Professors.GetByIdAsync(id, cancellationToken);
        if (professor is null)
            return Result<ProfessorDto>.Failure($"No se encontró el profesor con ID {id}.", "NOT_FOUND");

        return Result<ProfessorDto>.Success(MapToDto(professor));
    }

    public async Task<Result<IEnumerable<ProfessorDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var professors = await unitOfWork.Professors.GetAllWithSubjectsAsync(cancellationToken);
        return Result<IEnumerable<ProfessorDto>>.Success(professors.Select(MapToDto));
    }

    private static ProfessorDto MapToDto(Domain.Entities.Professor p) => new(
        p.Id, p.FirstName, p.LastName, p.FullName, p.Email, p.Bio,
        p.Subjects.Where(s => !s.IsDeleted)
            .Select(s => new SubjectSummaryDto(s.Id, s.Name, s.Code, s.Credits, p.Id, p.FullName))
            .ToList()
            .AsReadOnly());
}
