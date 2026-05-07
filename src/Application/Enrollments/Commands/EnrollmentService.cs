using Interrapidisimo.Application.Common.Interfaces;
using Interrapidisimo.Application.Common.Models;
using Interrapidisimo.Application.Enrollments.DTOs;
using Interrapidisimo.Application.Subjects.DTOs;
using Interrapidisimo.Domain.Exceptions;
using Interrapidisimo.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Interrapidisimo.Application.Enrollments.Commands;

public class EnrollmentService(IUnitOfWork unitOfWork, ILogger<EnrollmentService> logger) : IEnrollmentService
{
    private const string NotFoundCode = "NOT_FOUND";
    public async Task<Result<EnrollmentDto>> EnrollStudentAsync(EnrollStudentDto dto, CancellationToken cancellationToken = default)
    {
        var student = await unitOfWork.Students.GetByIdWithEnrollmentsNoTrackingAsync(dto.StudentId, cancellationToken);
        if (student is null)
            return Result<EnrollmentDto>.Failure($"No se encontró el estudiante con ID {dto.StudentId}.", NotFoundCode);

        var subject = await unitOfWork.Subjects.GetByIdWithProfessorAsync(dto.SubjectId, cancellationToken);
        if (subject is null)
            return Result<EnrollmentDto>.Failure($"No se encontró la materia con ID {dto.SubjectId}.", NotFoundCode);

        try
        {
            student.EnrollInSubject(subject);

            var enrollment = student.Enrollments.First(e => e.SubjectId == dto.SubjectId && !e.IsDeleted);
            await unitOfWork.Enrollments.AddAsync(enrollment, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Estudiante {StudentCode} inscrito en la materia {SubjectCode}",
                student.StudentCode, subject.Code);

            return Result<EnrollmentDto>.Success(new EnrollmentDto(
                enrollment.Id,
                student.Id,
                student.FullName,
                subject.Id,
                subject.Name,
                subject.Code,
                subject.Professor?.FullName ?? string.Empty,
                subject.Credits,
                enrollment.EnrolledAt));
        }
        catch (BusinessRuleException ex)
        {
            logger.LogWarning(ex, "Regla de negocio de inscripción violada: {Message}", ex.Message);
            return Result<EnrollmentDto>.Failure(ex.Message, "BUSINESS_RULE");
        }
    }

    public async Task<Result> WithdrawStudentAsync(WithdrawStudentDto dto, CancellationToken cancellationToken = default)
    {
        var student = await unitOfWork.Students.GetByIdWithEnrollmentsAsync(dto.StudentId, cancellationToken);
        if (student is null)
            return Result.Failure($"No se encontró el estudiante con ID {dto.StudentId}.", NotFoundCode);

        try
        {
            student.WithdrawFromSubject(dto.SubjectId);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Estudiante {StudentId} se retiró de la materia {SubjectId}", dto.StudentId, dto.SubjectId);
            return Result.Success();
        }
        catch (NotFoundException ex)
        {
            return Result.Failure(ex.Message, NotFoundCode);
        }
    }

    public async Task<Result<StudentEnrollmentStatusDto>> GetStudentEnrollmentStatusAsync(
        Guid studentId, CancellationToken cancellationToken = default)
    {
        var student = await unitOfWork.Students.GetByIdWithEnrollmentsAsync(studentId, cancellationToken);
        if (student is null)
            return Result<StudentEnrollmentStatusDto>.Failure($"No se encontró el estudiante con ID {studentId}.", NotFoundCode);

        var allSubjects = await unitOfWork.Subjects.GetAllWithDetailsAsync(cancellationToken);
        var activeEnrollments = student.Enrollments.Where(e => !e.IsDeleted).ToList();
        var enrolledSubjectIds = activeEnrollments.Select(e => e.SubjectId).ToHashSet();
        var enrolledProfessorIds = activeEnrollments
            .Select(e => e.Subject?.ProfessorId)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .ToHashSet();

        var enrolledSubjects = allSubjects
            .Where(s => enrolledSubjectIds.Contains(s.Id))
            .Select(s => new SubjectSummaryDto(s.Id, s.Name, s.Code, s.Credits, s.ProfessorId, s.Professor?.FullName ?? string.Empty))
            .ToList();

        var availableSubjects = allSubjects
            .Where(s => !enrolledSubjectIds.Contains(s.Id) && !enrolledProfessorIds.Contains(s.ProfessorId) && !s.IsDeleted)
            .Select(s => new SubjectSummaryDto(s.Id, s.Name, s.Code, s.Credits, s.ProfessorId, s.Professor?.FullName ?? string.Empty))
            .ToList();

        return Result<StudentEnrollmentStatusDto>.Success(new StudentEnrollmentStatusDto(
            student.Id,
            student.FullName,
            student.EnrolledSubjectsCount,
            Domain.Entities.Student.MaxSubjects,
            student.TotalCredits,
            enrolledSubjects.AsReadOnly(),
            availableSubjects.AsReadOnly()));
    }

    public async Task<Result<IEnumerable<SubjectEnrollmentSummaryDto>>> GetStudentEnrollmentsWithClassmatesAsync(
        Guid studentId, CancellationToken cancellationToken = default)
    {
        var student = await unitOfWork.Students.GetByIdWithEnrollmentsAsync(studentId, cancellationToken);
        if (student is null)
            return Result<IEnumerable<SubjectEnrollmentSummaryDto>>.Failure(
                $"No se encontró el estudiante con ID {studentId}.", NotFoundCode);

        var activeEnrollments = student.Enrollments.Where(e => !e.IsDeleted).ToList();
        var result = new List<SubjectEnrollmentSummaryDto>();

        foreach (var enrollment in activeEnrollments)
        {
            var subject = await unitOfWork.Subjects.GetByIdWithDetailsAsync(enrollment.SubjectId, cancellationToken);
            if (subject is null) continue;

            var classmates = await unitOfWork.Enrollments.GetClassmatesAsync(studentId, enrollment.SubjectId, cancellationToken);

            result.Add(new SubjectEnrollmentSummaryDto(
                subject.Id,
                subject.Name,
                subject.Code,
                subject.Professor?.FullName ?? string.Empty,
                subject.Credits,
                enrollment.EnrolledAt,
                classmates.Select(c => new ClassmateDto(c.Id, c.FullName)).ToList().AsReadOnly()));
        }

        return Result<IEnumerable<SubjectEnrollmentSummaryDto>>.Success(result);
    }
}
