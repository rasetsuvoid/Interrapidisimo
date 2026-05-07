using Interrapidisimo.Domain.Common;
using Interrapidisimo.Domain.Exceptions;

namespace Interrapidisimo.Domain.Entities;

public class Student : AuditableEntity
{
    public const int MaxSubjects = 3;
    public const int CreditsPerSubject = 3;

    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string StudentCode { get; private set; } = string.Empty;
    public string? PhoneNumber { get; private set; }
    public DateTime DateOfBirth { get; private set; }

    private readonly List<StudentSubject> _enrollments = [];
    public IReadOnlyCollection<StudentSubject> Enrollments => _enrollments.AsReadOnly();

    public int TotalCredits => _enrollments.Count(e => !e.IsDeleted) * CreditsPerSubject;
    public int EnrolledSubjectsCount => _enrollments.Count(e => !e.IsDeleted);
    public string FullName => $"{FirstName} {LastName}";

    private Student() { }

    public static Student Create(
        string firstName,
        string lastName,
        string email,
        string studentCode,
        DateTime dateOfBirth,
        string? phoneNumber = null,
        string? createdBy = null)
    {
        var student = new Student
        {
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            StudentCode = studentCode.Trim().ToUpperInvariant(),
            DateOfBirth = dateOfBirth,
            PhoneNumber = phoneNumber?.Trim()
        };
        student.SetCreated(createdBy);
        return student;
    }

    public void Update(
        string firstName,
        string lastName,
        string email,
        string? phoneNumber,
        DateTime dateOfBirth,
        string? updatedBy = null)
    {
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email.Trim().ToLowerInvariant();
        PhoneNumber = phoneNumber?.Trim();
        DateOfBirth = dateOfBirth;
        SetUpdated(updatedBy);
    }

    public void EnrollInSubject(Subject subject, string? enrolledBy = null)
    {
        var activeEnrollments = _enrollments.Where(e => !e.IsDeleted).ToList();

        if (activeEnrollments.Count >= MaxSubjects)
            throw new BusinessRuleException(
                $"El estudiante no puede inscribirse en más de {MaxSubjects} materias. Actuales: {activeEnrollments.Count}");

        var alreadyEnrolled = activeEnrollments.Any(e => e.SubjectId == subject.Id);
        if (alreadyEnrolled)
            throw new BusinessRuleException($"El estudiante ya está inscrito en la materia '{subject.Name}'.");

        var sameProfessorConflict = activeEnrollments
            .Any(e => e.Subject?.ProfessorId == subject.ProfessorId);
        if (sameProfessorConflict)
            throw new BusinessRuleException(
                $"El estudiante no puede inscribirse en dos materias del mismo profesor.");

        var enrollment = StudentSubject.Create(Id, subject.Id, createdBy: enrolledBy);
        _enrollments.Add(enrollment);
    }

    public void WithdrawFromSubject(Guid subjectId, string? updatedBy = null)
    {
        var enrollment = _enrollments.FirstOrDefault(e => e.SubjectId == subjectId && !e.IsDeleted)
            ?? throw new NotFoundException($"No se encontró la inscripción para la materia {subjectId}.");

        enrollment.MarkAsDeleted(updatedBy);
        SetUpdated(updatedBy);
    }
}
