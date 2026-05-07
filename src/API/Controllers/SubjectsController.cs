using Interrapidisimo.Application.Common.Interfaces;
using Interrapidisimo.Application.Common.Models;
using Interrapidisimo.Application.Subjects.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Interrapidisimo.API.Controllers;

[Authorize]
public class SubjectsController(ISubjectService subjectService) : BaseController
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SubjectDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        var result = await subjectService.GetAllAsync(cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<SubjectDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<SubjectDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await subjectService.GetByIdAsync(id, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("by-professor/{professorId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SubjectDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByProfessor(Guid professorId, CancellationToken cancellationToken = default)
    {
        var result = await subjectService.GetByProfessorAsync(professorId, cancellationToken);
        return HandleResult(result);
    }
}
