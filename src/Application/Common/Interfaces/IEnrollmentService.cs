using Interrapidisimo.Application.Common.Models;
using Interrapidisimo.Application.Enrollments.DTOs;

namespace Interrapidisimo.Application.Common.Interfaces;

public interface IEnrollmentService
{
    Task<Result<EnrollmentDto>> EnrollStudentAsync(EnrollStudentDto dto, CancellationToken cancellationToken = default);
    Task<Result> WithdrawStudentAsync(WithdrawStudentDto dto, CancellationToken cancellationToken = default);
    Task<Result<StudentEnrollmentStatusDto>> GetStudentEnrollmentStatusAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<SubjectEnrollmentSummaryDto>>> GetStudentEnrollmentsWithClassmatesAsync(Guid studentId, CancellationToken cancellationToken = default);
}
