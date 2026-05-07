using Interrapidisimo.Application.Common.Interfaces;
using Interrapidisimo.Application.Common.Models;
using Interrapidisimo.Application.Students.DTOs;
using Interrapidisimo.Domain.Entities;
using Interrapidisimo.Domain.Exceptions;
using Interrapidisimo.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Interrapidisimo.Application.Students.Commands;

public class StudentService(IUnitOfWork unitOfWork, ILogger<StudentService> logger) : IStudentService
{
    public async Task<Result<StudentDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var student = await unitOfWork.Students.GetByIdWithEnrollmentsAsync(id, cancellationToken);
        if (student is null)
            return Result<StudentDto>.Failure($"No se encontró el estudiante con ID {id}.", "NOT_FOUND");

        return Result<StudentDto>.Success(MapToDto(student));
    }

    public async Task<Result<PagedResult<StudentDto>>> GetPagedAsync(
        int page, int pageSize, string? search = null, CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var (items, totalCount) = await unitOfWork.Students.GetPagedAsync(page, pageSize, search, cancellationToken);
        var dtos = items.Select(MapToDto);
        return Result<PagedResult<StudentDto>>.Success(PagedResult<StudentDto>.Create(dtos, totalCount, page, pageSize));
    }

    public async Task<Result<IEnumerable<StudentDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var students = await unitOfWork.Students.GetAllAsync(cancellationToken);
        return Result<IEnumerable<StudentDto>>.Success(students.Select(MapToDto));
    }

    public async Task<Result<StudentDto>> CreateAsync(CreateStudentDto dto, CancellationToken cancellationToken = default)
    {
        if (await unitOfWork.Students.EmailExistsAsync(dto.Email, cancellationToken: cancellationToken))
            return Result<StudentDto>.Failure($"Ya existe un estudiante con el correo '{dto.Email}'.", "CONFLICT");

        if (await unitOfWork.Students.CodeExistsAsync(dto.StudentCode, cancellationToken: cancellationToken))
            return Result<StudentDto>.Failure($"Ya existe un estudiante con el código '{dto.StudentCode}'.", "CONFLICT");

        var student = Student.Create(
            dto.FirstName, dto.LastName, dto.Email,
            dto.StudentCode, dto.DateOfBirth, dto.PhoneNumber);

        await unitOfWork.Students.AddAsync(student, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Estudiante creado: {StudentCode} - {FullName}", student.StudentCode, student.FullName);
        return Result<StudentDto>.Success(MapToDto(student));
    }

    public async Task<Result<StudentDto>> UpdateAsync(Guid id, UpdateStudentDto dto, CancellationToken cancellationToken = default)
    {
        var student = await unitOfWork.Students.GetByIdWithEnrollmentsAsync(id, cancellationToken);
        if (student is null)
            return Result<StudentDto>.Failure($"No se encontró el estudiante con ID {id}.", "NOT_FOUND");

        if (await unitOfWork.Students.EmailExistsAsync(dto.Email, id, cancellationToken))
            return Result<StudentDto>.Failure($"Ya existe un estudiante con el correo '{dto.Email}'.", "CONFLICT");

        student.Update(dto.FirstName, dto.LastName, dto.Email, dto.PhoneNumber, dto.DateOfBirth);
        unitOfWork.Students.Update(student);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Estudiante actualizado: {StudentId}", id);
        return Result<StudentDto>.Success(MapToDto(student));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var student = await unitOfWork.Students.GetByIdAsync(id, cancellationToken);
        if (student is null)
            return Result.Failure($"No se encontró el estudiante con ID {id}.", "NOT_FOUND");

        student.MarkAsDeleted();
        unitOfWork.Students.Delete(student);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Estudiante eliminado: {StudentId}", id);
        return Result.Success();
    }

    private static StudentDto MapToDto(Student s) => new(
        s.Id, s.FirstName, s.LastName, s.FullName,
        s.Email, s.StudentCode, s.PhoneNumber,
        s.DateOfBirth, s.EnrolledSubjectsCount,
        s.TotalCredits, s.CreatedAt, s.UpdatedAt);
}
