using Interrapidisimo.Application.Subjects.DTOs;

namespace Interrapidisimo.Application.Enrollments.DTOs;

public record EnrollmentDto(
    Guid Id,
    Guid StudentId,
    string StudentName,
    Guid SubjectId,
    string SubjectName,
    string SubjectCode,
    string ProfessorName,
    int Credits,
    DateTime EnrolledAt
);

public record EnrollStudentDto(
    Guid StudentId,
    Guid SubjectId
);

public record WithdrawStudentDto(
    Guid StudentId,
    Guid SubjectId
);

public record ClassmateDto(
    Guid StudentId,
    string FullName
);

public record SubjectEnrollmentSummaryDto(
    Guid SubjectId,
    string SubjectName,
    string SubjectCode,
    string ProfessorName,
    int Credits,
    DateTime EnrolledAt,
    IReadOnlyList<ClassmateDto> Classmates
);

public record StudentEnrollmentStatusDto(
    Guid StudentId,
    string StudentName,
    int EnrolledSubjectsCount,
    int MaxSubjects,
    int TotalCredits,
    IReadOnlyList<SubjectSummaryDto> EnrolledSubjects,
    IReadOnlyList<SubjectSummaryDto> AvailableSubjects
);
