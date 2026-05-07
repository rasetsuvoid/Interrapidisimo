namespace Interrapidisimo.Application.Subjects.DTOs;

public record SubjectDto(
    Guid Id,
    string Name,
    string Code,
    string? Description,
    int Credits,
    Guid ProfessorId,
    string ProfessorName,
    string ProfessorEmail,
    int EnrolledStudentsCount
);

public record SubjectSummaryDto(
    Guid Id,
    string Name,
    string Code,
    int Credits,
    Guid ProfessorId,
    string ProfessorName
);
