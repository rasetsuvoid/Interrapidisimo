using Interrapidisimo.Application.Students.DTOs;

namespace Interrapidisimo.Application.Auth.DTOs;

public record RegisterStudentDto(
    string FirstName,
    string LastName,
    string Email,
    string StudentCode,
    DateTime DateOfBirth,
    string? PhoneNumber,
    string Password
);

public record LoginDto(
    string Email,
    string Password
);

public record AuthResponseDto(
    string Token,
    DateTime ExpiresAt,
    StudentDto Student
);
