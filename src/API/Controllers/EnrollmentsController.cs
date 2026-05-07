using FluentValidation;
using Interrapidisimo.Application.Common.Interfaces;
using Interrapidisimo.Application.Common.Models;
using Interrapidisimo.Application.Enrollments.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Interrapidisimo.API.Controllers;

[Authorize]
public class EnrollmentsController(
    IEnrollmentService enrollmentService,
    IValidator<EnrollStudentDto> enrollValidator) : BaseController
{
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentDto>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Enroll([FromBody] EnrollStudentDto dto, CancellationToken cancellationToken = default)
    {
        var validation = await enrollValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return BadRequest(ApiResponse<EnrollmentDto>.Fail(
                validation.Errors.Select(e => e.ErrorMessage), HttpContext.TraceIdentifier));

        var result = await enrollmentService.EnrollStudentAsync(dto, cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Withdraw([FromBody] WithdrawStudentDto dto, CancellationToken cancellationToken = default)
    {
        var result = await enrollmentService.WithdrawStudentAsync(dto, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("student/{studentId:guid}/status")]
    [ProducesResponseType(typeof(ApiResponse<StudentEnrollmentStatusDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatus(Guid studentId, CancellationToken cancellationToken = default)
    {
        var result = await enrollmentService.GetStudentEnrollmentStatusAsync(studentId, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("student/{studentId:guid}/classmates")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SubjectEnrollmentSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetClassmates(Guid studentId, CancellationToken cancellationToken = default)
    {
        var result = await enrollmentService.GetStudentEnrollmentsWithClassmatesAsync(studentId, cancellationToken);
        return HandleResult(result);
    }
}
