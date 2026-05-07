using Interrapidisimo.Application.Subjects.DTOs;

namespace Interrapidisimo.Application.Professors.DTOs;

public record ProfessorDto(
    Guid Id,
    string FirstName,
    string LastName,
    string FullName,
    string Email,
    string? Bio,
    IReadOnlyList<SubjectSummaryDto> Subjects
);

public record ProfessorSummaryDto(
    Guid Id,
    string FullName,
    string Email
);
