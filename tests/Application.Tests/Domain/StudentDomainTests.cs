using FluentAssertions;
using Interrapidisimo.Domain.Entities;
using Interrapidisimo.Domain.Exceptions;

namespace Interrapidisimo.Application.Tests.Domain;

[TestClass]
public class StudentDomainTests
{
    private static Professor CreateProfessor(string? name = null) =>
        Professor.Create(name ?? "Test", "Professor", $"{Guid.NewGuid()}@test.com");

    private static Subject CreateSubject(Guid professorId, string? code = null) =>
        Subject.Create($"Subject {code ?? "X"}", code ?? "SUB-001", professorId);

    private static Student CreateStudent() =>
        Student.Create("John", "Doe", "john@test.com", "STU-001", DateTime.UtcNow.AddYears(-20));

    [TestMethod]
    public void Student_Create_ShouldSetPropertiesCorrectly()
    {
        var student = CreateStudent();

        student.FirstName.Should().Be("John");
        student.LastName.Should().Be("Doe");
        student.Email.Should().Be("john@test.com");
        student.StudentCode.Should().Be("STU-001");
        student.EnrolledSubjectsCount.Should().Be(0);
        student.TotalCredits.Should().Be(0);
    }

    [TestMethod]
    public void Student_EnrollInSubject_ShouldAddEnrollment()
    {
        var professor = CreateProfessor();
        var subject = CreateSubject(professor.Id, "SUB-001");
        var student = CreateStudent();

        student.EnrollInSubject(subject);

        student.EnrolledSubjectsCount.Should().Be(1);
        student.TotalCredits.Should().Be(Subject.CreditsValue);
    }

    [TestMethod]
    public void Student_EnrollInSubject_ShouldThrow_WhenMaxSubjectsExceeded()
    {
        var student = CreateStudent();
        var prof1 = CreateProfessor("Prof1");
        var prof2 = CreateProfessor("Prof2");
        var prof3 = CreateProfessor("Prof3");
        var prof4 = CreateProfessor("Prof4");

        student.EnrollInSubject(CreateSubject(prof1.Id, "S001"));
        student.EnrollInSubject(CreateSubject(prof2.Id, "S002"));
        student.EnrollInSubject(CreateSubject(prof3.Id, "S003"));

        var fourthSubject = CreateSubject(prof4.Id, "S004");

        var act = () => student.EnrollInSubject(fourthSubject);

        act.Should().Throw<BusinessRuleException>()
           .WithMessage($"*{Student.MaxSubjects}*");
    }

    [TestMethod]
    public void Student_EnrollInSubject_ShouldThrow_WhenAlreadyEnrolled()
    {
        var professor = CreateProfessor();
        var subject = CreateSubject(professor.Id, "S001");
        var student = CreateStudent();

        student.EnrollInSubject(subject);

        var act = () => student.EnrollInSubject(subject);

        act.Should().Throw<BusinessRuleException>()
           .WithMessage("*ya está inscrito*");
    }

    [TestMethod]
    public void Student_EnrollInSubject_ShouldThrow_WhenSameProfessorConflict()
    {
        var professor = CreateProfessor();
        var subject1 = CreateSubject(professor.Id, "S001");
        var subject2 = CreateSubject(professor.Id, "S002");
        var student = CreateStudent();

        student.EnrollInSubject(subject1);

        var act = () => student.EnrollInSubject(subject2);

        act.Should().Throw<BusinessRuleException>()
           .WithMessage("*mismo profesor*");
    }

    [TestMethod]
    public void Student_EnrollInThreeSubjects_WithDifferentProfessors_ShouldSucceed()
    {
        var student = CreateStudent();
        var prof1 = CreateProfessor("P1");
        var prof2 = CreateProfessor("P2");
        var prof3 = CreateProfessor("P3");

        student.EnrollInSubject(CreateSubject(prof1.Id, "S001"));
        student.EnrollInSubject(CreateSubject(prof2.Id, "S002"));
        student.EnrollInSubject(CreateSubject(prof3.Id, "S003"));

        student.EnrolledSubjectsCount.Should().Be(3);
        student.TotalCredits.Should().Be(9);
    }

    [TestMethod]
    public void Student_WithdrawFromSubject_ShouldRemoveEnrollment()
    {
        var professor = CreateProfessor();
        var subject = CreateSubject(professor.Id, "S001");
        var student = CreateStudent();

        student.EnrollInSubject(subject);
        student.WithdrawFromSubject(subject.Id);

        student.EnrolledSubjectsCount.Should().Be(0);
        student.TotalCredits.Should().Be(0);
    }

    [TestMethod]
    public void Student_WithdrawFromSubject_ShouldThrow_WhenNotEnrolled()
    {
        var student = CreateStudent();
        var randomId = Guid.NewGuid();

        var act = () => student.WithdrawFromSubject(randomId);

        act.Should().Throw<NotFoundException>();
    }

    [TestMethod]
    public void Student_FullName_ShouldConcatenateCorrectly()
    {
        var student = Student.Create("María", "García López", "m@test.com", "STU-002", DateTime.UtcNow.AddYears(-22));

        student.FullName.Should().Be("María García López");
    }

    [TestMethod]
    public void Student_Email_ShouldBeLowercase()
    {
        var student = Student.Create("Test", "User", "TEST@EXAMPLE.COM", "STU-003", DateTime.UtcNow.AddYears(-20));

        student.Email.Should().Be("test@example.com");
    }

    [TestMethod]
    public void Student_StudentCode_ShouldBeUppercase()
    {
        var student = Student.Create("Test", "User", "t@test.com", "stu-004", DateTime.UtcNow.AddYears(-20));

        student.StudentCode.Should().Be("STU-004");
    }

    [TestMethod]
    public void Subject_Credits_ShouldAlwaysBe3()
    {
        var prof = CreateProfessor();
        var subject = CreateSubject(prof.Id);

        subject.Credits.Should().Be(3);
    }

    [TestMethod]
    public void Student_After3Enrollments_TotalCreditsShouldBe9()
    {
        var student = CreateStudent();
        var p1 = CreateProfessor("P1");
        var p2 = CreateProfessor("P2");
        var p3 = CreateProfessor("P3");

        student.EnrollInSubject(CreateSubject(p1.Id, "A1"));
        student.EnrollInSubject(CreateSubject(p2.Id, "A2"));
        student.EnrollInSubject(CreateSubject(p3.Id, "A3"));

        student.TotalCredits.Should().Be(9);
        (student.TotalCredits % 3).Should().Be(0);
    }
}
