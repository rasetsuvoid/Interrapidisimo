using FluentValidation;
using Interrapidisimo.Application.Auth.DTOs;
using Interrapidisimo.Application.Common.Interfaces;
using Interrapidisimo.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Interrapidisimo.API.Controllers;

public record EncryptedRegisterRequest(
    string FirstName,
    string LastName,
    string Email,
    string StudentCode,
    DateTime DateOfBirth,
    string? PhoneNumber,
    string EncryptedPassword);

public record EncryptedLoginRequest(
    string Email,
    string EncryptedPassword);

[AllowAnonymous]
[EnableRateLimiting("auth")]
public class AuthController(
    IAuthService authService,
    IRsaEncryptionService rsaService,
    IValidator<RegisterStudentDto> registerValidator,
    IValidator<LoginDto> loginValidator) : BaseController
{
    [HttpGet("public-key")]
    [DisableRateLimiting]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public IActionResult GetPublicKey() =>
        Ok(ApiResponse<object>.Ok(
            new { publicKey = rsaService.GetPublicKeyBase64() },
            traceId: HttpContext.TraceIdentifier));

    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] EncryptedRegisterRequest request, CancellationToken cancellationToken = default)
    {
        string plainPassword;
        try { plainPassword = rsaService.Decrypt(request.EncryptedPassword); }
        catch { return BadRequest(ApiResponse<AuthResponseDto>.Fail("Contraseña inválida o mal cifrada.", HttpContext.TraceIdentifier)); }

        var dto = new RegisterStudentDto(
            request.FirstName, request.LastName, request.Email,
            request.StudentCode, request.DateOfBirth, request.PhoneNumber,
            plainPassword);

        var validation = await registerValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return BadRequest(ApiResponse<AuthResponseDto>.Fail(
                validation.Errors.Select(e => e.ErrorMessage), HttpContext.TraceIdentifier));

        var result = await authService.RegisterStudentAsync(dto, cancellationToken);
        if (result.IsFailure) return HandleAuthResult(result);

        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<AuthResponseDto>.Ok(result.Value!, "Registro completado correctamente.", HttpContext.TraceIdentifier));
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] EncryptedLoginRequest request, CancellationToken cancellationToken = default)
    {
        string plainPassword;
        try { plainPassword = rsaService.Decrypt(request.EncryptedPassword); }
        catch { return BadRequest(ApiResponse<AuthResponseDto>.Fail("Contraseña inválida o mal cifrada.", HttpContext.TraceIdentifier)); }

        var dto = new LoginDto(request.Email, plainPassword);

        var validation = await loginValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return BadRequest(ApiResponse<AuthResponseDto>.Fail(
                validation.Errors.Select(e => e.ErrorMessage), HttpContext.TraceIdentifier));

        var result = await authService.LoginAsync(dto, cancellationToken);
        return HandleAuthResult(result);
    }

    private IActionResult HandleAuthResult(Result<AuthResponseDto> result)
    {
        if (result.IsSuccess) return Ok(ApiResponse<AuthResponseDto>.Ok(result.Value!, traceId: HttpContext.TraceIdentifier));

        return result.ErrorCode switch
        {
            "CONFLICT"     => Conflict(ApiResponse<AuthResponseDto>.Fail(result.Error!, HttpContext.TraceIdentifier)),
            "UNAUTHORIZED" => Unauthorized(ApiResponse<AuthResponseDto>.Fail(result.Error!, HttpContext.TraceIdentifier)),
            _ => BadRequest(ApiResponse<AuthResponseDto>.Fail(result.Errors.Any() ? result.Errors : [result.Error!], HttpContext.TraceIdentifier))
        };
    }
}
