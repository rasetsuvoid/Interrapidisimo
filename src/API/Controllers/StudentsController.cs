using FluentValidation;
using Interrapidisimo.Application.Common.Interfaces;
using Interrapidisimo.Application.Common.Models;
using Interrapidisimo.Application.Students.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Interrapidisimo.API.Controllers;

[Authorize]
public class StudentsController(
    IStudentService studentService,
    IValidator<CreateStudentDto> createValidator,
    IValidator<UpdateStudentDto> updateValidator) : BaseController
{
    [HttpGet(Name = "GetStudents")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<StudentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var result = await studentService.GetPagedAsync(page, pageSize, search, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id:guid}", Name = "GetStudentById")]
    [ProducesResponseType(typeof(ApiResponse<StudentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<StudentDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await studentService.GetByIdAsync(id, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<StudentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<StudentDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<StudentDto>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateStudentDto dto, CancellationToken cancellationToken = default)
    {
        var validation = await createValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return BadRequest(ApiResponse<StudentDto>.Fail(
                validation.Errors.Select(e => e.ErrorMessage), HttpContext.TraceIdentifier));

        var result = await studentService.CreateAsync(dto, cancellationToken);
        if (result.IsFailure) return HandleResult(result);

        return CreatedAtRoute("GetStudentById",
            new { id = result.Value!.Id },
            ApiResponse<StudentDto>.Ok(result.Value, "Estudiante creado exitosamente.", HttpContext.TraceIdentifier));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<StudentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<StudentDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStudentDto dto, CancellationToken cancellationToken = default)
    {
        var validation = await updateValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return BadRequest(ApiResponse<StudentDto>.Fail(
                validation.Errors.Select(e => e.ErrorMessage), HttpContext.TraceIdentifier));

        var result = await studentService.UpdateAsync(id, dto, cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await studentService.DeleteAsync(id, cancellationToken);
        return HandleResult(result);
    }
}
