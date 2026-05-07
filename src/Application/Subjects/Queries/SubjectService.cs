using Interrapidisimo.Application.Common.Interfaces;
using Interrapidisimo.Application.Common.Models;
using Interrapidisimo.Application.Subjects.DTOs;
using Interrapidisimo.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Interrapidisimo.Application.Subjects.Queries;

public class SubjectService(IUnitOfWork unitOfWork, ILogger<SubjectService> logger) : ISubjectService
{
    public async Task<Result<SubjectDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var subject = await unitOfWork.Subjects.GetByIdWithDetailsAsync(id, cancellationToken);
        if (subject is null)
            return Result<SubjectDto>.Failure($"No se encontró la materia con ID {id}.", "NOT_FOUND");

        return Result<SubjectDto>.Success(MapToDto(subject));
    }

    public async Task<Result<IEnumerable<SubjectDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var subjects = await unitOfWork.Subjects.GetAllWithDetailsAsync(cancellationToken);
        return Result<IEnumerable<SubjectDto>>.Success(subjects.Select(MapToDto));
    }

    public async Task<Result<IEnumerable<SubjectDto>>> GetByProfessorAsync(Guid professorId, CancellationToken cancellationToken = default)
    {
        var subjects = await unitOfWork.Subjects.GetByProfessorAsync(professorId, cancellationToken);
        var subjectsWithDetails = new List<Domain.Entities.Subject>();
        foreach (var s in subjects)
        {
            var detailed = await unitOfWork.Subjects.GetByIdWithDetailsAsync(s.Id, cancellationToken);
            if (detailed is not null) subjectsWithDetails.Add(detailed);
        }
        return Result<IEnumerable<SubjectDto>>.Success(subjectsWithDetails.Select(MapToDto));
    }

    private static SubjectDto MapToDto(Domain.Entities.Subject s) => new(
        s.Id, s.Name, s.Code, s.Description, s.Credits,
        s.ProfessorId,
        s.Professor?.FullName ?? string.Empty,
        s.Professor?.Email ?? string.Empty,
        s.EnrolledStudentsCount);
}
