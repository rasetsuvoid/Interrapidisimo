using Interrapidisimo.Application.Auth.DTOs;
using Interrapidisimo.Application.Common.Interfaces;
using Interrapidisimo.Application.Common.Models;
using Interrapidisimo.Application.Students.DTOs;
using Interrapidisimo.Domain.Entities;
using Interrapidisimo.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Interrapidisimo.Application.Auth.Commands;

public class AuthService(
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwtTokenService,
    ILogger<AuthService> logger) : IAuthService
{
    public async Task<Result<AuthResponseDto>> RegisterStudentAsync(RegisterStudentDto dto, CancellationToken cancellationToken = default)
    {
        if (await unitOfWork.Students.EmailExistsAsync(dto.Email, cancellationToken: cancellationToken))
            return Result<AuthResponseDto>.Failure($"Ya existe un estudiante con el correo '{dto.Email}'.", "CONFLICT");

        if (await unitOfWork.Students.CodeExistsAsync(dto.StudentCode, cancellationToken: cancellationToken))
            return Result<AuthResponseDto>.Failure($"Ya existe un estudiante con el código '{dto.StudentCode}'.", "CONFLICT");

        var student = Student.Create(
            dto.FirstName,
            dto.LastName,
            dto.Email,
            dto.StudentCode,
            dto.DateOfBirth,
            dto.PhoneNumber,
            "self-register");

        var account = StudentAccount.Create(
            student.Id,
            passwordHasher.Hash(dto.Password),
            "self-register");

        await unitOfWork.Students.AddAsync(student, cancellationToken);
        await unitOfWork.StudentAccounts.AddAsync(account, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var (token, expiresAt) = jwtTokenService.GenerateToken(student);

        logger.LogInformation("Cuenta de estudiante registrada: {StudentCode}", student.StudentCode);

        return Result<AuthResponseDto>.Success(new AuthResponseDto(
            token,
            expiresAt,
            MapToDto(student)));
    }

    public async Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
    {
        var account = await unitOfWork.StudentAccounts.GetByEmailAsync(dto.Email, cancellationToken);
        if (account?.Student is null || !passwordHasher.Verify(dto.Password, account.PasswordHash))
            return Result<AuthResponseDto>.Failure("Credenciales inválidas.", "UNAUTHORIZED");

        var (token, expiresAt) = jwtTokenService.GenerateToken(account.Student);

        logger.LogInformation("Inicio de sesión de estudiante: {StudentId}", account.Student.Id);

        return Result<AuthResponseDto>.Success(new AuthResponseDto(
            token,
            expiresAt,
            MapToDto(account.Student)));
    }

    private static StudentDto MapToDto(Student s) => new(
        s.Id,
        s.FirstName,
        s.LastName,
        s.FullName,
        s.Email,
        s.StudentCode,
        s.PhoneNumber,
        s.DateOfBirth,
        s.EnrolledSubjectsCount,
        s.TotalCredits,
        s.CreatedAt,
        s.UpdatedAt);
}
