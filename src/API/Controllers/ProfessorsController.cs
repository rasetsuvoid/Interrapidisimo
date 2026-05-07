using Interrapidisimo.Application.Common.Interfaces;
using Interrapidisimo.Application.Common.Models;
using Interrapidisimo.Application.Professors.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Interrapidisimo.API.Controllers;

[Authorize]
public class ProfessorsController(IProfessorService professorService) : BaseController
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProfessorDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        var result = await professorService.GetAllAsync(cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProfessorDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ProfessorDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await professorService.GetByIdAsync(id, cancellationToken);
        return HandleResult(result);
    }
}
