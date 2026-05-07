using FluentAssertions;
using Interrapidisimo.Application.Enrollments.Commands;
using Interrapidisimo.Application.Enrollments.DTOs;
using Interrapidisimo.Domain.Entities;
using Interrapidisimo.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Interrapidisimo.Application.Tests.Enrollments;

[TestClass]
public class EnrollmentServiceTests
{
    private Mock<IUnitOfWork> _unitOfWork = null!;
    private Mock<IStudentRepository> _studentRepo = null!;
    private Mock<ISubjectRepository> _subjectRepo = null!;
    private Mock<IEnrollmentRepository> _enrollmentRepo = null!;
    private Mock<ILogger<EnrollmentService>> _logger = null!;
    private EnrollmentService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _unitOfWork = new Mock<IUnitOfWork>();
        _studentRepo = new Mock<IStudentRepository>();
        _subjectRepo = new Mock<ISubjectRepository>();
        _enrollmentRepo = new Mock<IEnrollmentRepository>();
        _logger = new Mock<ILogger<EnrollmentService>>();

        _unitOfWork.Setup(u => u.Students).Returns(_studentRepo.Object);
        _unitOfWork.Setup(u => u.Subjects).Returns(_subjectRepo.Object);
        _unitOfWork.Setup(u => u.Enrollments).Returns(_enrollmentRepo.Object);
        _unitOfWork.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);

        _service = new EnrollmentService(_unitOfWork.Object, _logger.Object);
    }

    [TestMethod]
    public async Task EnrollStudentAsync_ShouldFail_WhenStudentNotFound()
    {
        _studentRepo.Setup(r => r.GetByIdWithEnrollmentsAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync((Student?)null);

        var result = await _service.EnrollStudentAsync(new EnrollStudentDto(Guid.NewGuid(), Guid.NewGuid()));

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("NOT_FOUND");
    }

    [TestMethod]
    public async Task EnrollStudentAsync_ShouldFail_WhenSubjectNotFound()
    {
        var student = Student.Create("John", "Doe", "john@test.com", "STU-001", DateTime.UtcNow.AddYears(-20));
        _studentRepo.Setup(r => r.GetByIdWithEnrollmentsAsync(student.Id, default)).ReturnsAsync(student);
        _subjectRepo.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), default)).ReturnsAsync((Subject?)null);

        var result = await _service.EnrollStudentAsync(new EnrollStudentDto(student.Id, Guid.NewGuid()));

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("NOT_FOUND");
    }

    [TestMethod]
    public async Task EnrollStudentAsync_ShouldFail_WithBusinessRule_WhenExceedsMaxSubjects()
    {
        var student = Student.Create("John", "Doe", "john@test.com", "STU-001", DateTime.UtcNow.AddYears(-20));
        var p1 = Professor.Create("P1", "X", "p1@t.com");
        var p2 = Professor.Create("P2", "X", "p2@t.com");
        var p3 = Professor.Create("P3", "X", "p3@t.com");
        var p4 = Professor.Create("P4", "X", "p4@t.com");

        student.EnrollInSubject(Subject.Create("S1", "S001", p1.Id));
        student.EnrollInSubject(Subject.Create("S2", "S002", p2.Id));
        student.EnrollInSubject(Subject.Create("S3", "S003", p3.Id));

        var newSubject = Subject.Create("S4", "S004", p4.Id);

        _studentRepo.Setup(r => r.GetByIdWithEnrollmentsNoTrackingAsync(student.Id, default)).ReturnsAsync(student);
        _subjectRepo.Setup(r => r.GetByIdWithProfessorAsync(newSubject.Id, default)).ReturnsAsync(newSubject);

        var result = await _service.EnrollStudentAsync(new EnrollStudentDto(student.Id, newSubject.Id));

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("BUSINESS_RULE");
        result.Error.Should().Contain("3");
    }

    [TestMethod]
    public async Task EnrollStudentAsync_ShouldFail_WithBusinessRule_WhenSameProfessor()
    {
        var student = Student.Create("John", "Doe", "john@test.com", "STU-001", DateTime.UtcNow.AddYears(-20));
        var professor = Professor.Create("Prof", "Test", "prof@t.com");
        var subject1 = Subject.Create("S1", "S001", professor.Id);

        student.EnrollInSubject(subject1);

        var subject2 = Subject.Create("S2", "S002", professor.Id);

        _studentRepo.Setup(r => r.GetByIdWithEnrollmentsNoTrackingAsync(student.Id, default)).ReturnsAsync(student);
        _subjectRepo.Setup(r => r.GetByIdWithProfessorAsync(subject2.Id, default)).ReturnsAsync(subject2);

        var result = await _service.EnrollStudentAsync(new EnrollStudentDto(student.Id, subject2.Id));

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("BUSINESS_RULE");
        result.Error.Should().Contain("profesor");
    }

    [TestMethod]
    public async Task GetStudentEnrollmentStatusAsync_ShouldReturnCorrectStatus()
    {
        var student = Student.Create("John", "Doe", "john@test.com", "STU-001", DateTime.UtcNow.AddYears(-20));
        var subjects = new List<Subject>();

        _studentRepo.Setup(r => r.GetByIdWithEnrollmentsAsync(student.Id, default)).ReturnsAsync(student);
        _subjectRepo.Setup(r => r.GetAllWithDetailsAsync(default)).ReturnsAsync(subjects);

        var result = await _service.GetStudentEnrollmentStatusAsync(student.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value!.MaxSubjects.Should().Be(Student.MaxSubjects);
        result.Value.EnrolledSubjectsCount.Should().Be(0);
        result.Value.TotalCredits.Should().Be(0);
    }
}
