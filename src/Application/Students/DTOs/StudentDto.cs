namespace Interrapidisimo.Application.Students.DTOs;

public record StudentDto(
    Guid Id,
    string FirstName,
    string LastName,
    string FullName,
    string Email,
    string StudentCode,
    string? PhoneNumber,
    DateTime DateOfBirth,
    int EnrolledSubjectsCount,
    int TotalCredits,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record StudentSummaryDto(
    Guid Id,
    string FullName,
    string StudentCode,
    string Email
);

public record CreateStudentDto(
    string FirstName,
    string LastName,
    string Email,
    string StudentCode,
    DateTime DateOfBirth,
    string? PhoneNumber
);

public record UpdateStudentDto(
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    DateTime DateOfBirth
);
