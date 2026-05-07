using FluentAssertions;
using Interrapidisimo.Application.Students.Commands;
using Interrapidisimo.Application.Students.DTOs;
using Interrapidisimo.Domain.Entities;
using Interrapidisimo.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Interrapidisimo.Application.Tests.Students;

[TestClass]
public class StudentServiceTests
{
    private Mock<IUnitOfWork> _unitOfWork = null!;
    private Mock<IStudentRepository> _studentRepo = null!;
    private Mock<ILogger<StudentService>> _logger = null!;
    private StudentService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _unitOfWork = new Mock<IUnitOfWork>();
        _studentRepo = new Mock<IStudentRepository>();
        _logger = new Mock<ILogger<StudentService>>();

        _unitOfWork.Setup(u => u.Students).Returns(_studentRepo.Object);
        _unitOfWork.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);

        _service = new StudentService(_unitOfWork.Object, _logger.Object);
    }

    [TestMethod]
    public async Task GetByIdAsync_ShouldReturnStudent_WhenExists()
    {
        var student = Student.Create("John", "Doe", "john@test.com", "STU-001", DateTime.UtcNow.AddYears(-20));
        _studentRepo.Setup(r => r.GetByIdWithEnrollmentsAsync(student.Id, default)).ReturnsAsync(student);

        var result = await _service.GetByIdAsync(student.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Email.Should().Be("john@test.com");
        result.Value.FullName.Should().Be("John Doe");
    }

    [TestMethod]
    public async Task GetByIdAsync_ShouldReturnFailure_WhenNotFound()
    {
        _studentRepo.Setup(r => r.GetByIdWithEnrollmentsAsync(It.IsAny<Guid>(), default)).ReturnsAsync((Student?)null);

        var result = await _service.GetByIdAsync(Guid.NewGuid());

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("NOT_FOUND");
    }

    [TestMethod]
    public async Task CreateAsync_ShouldSucceed_WithValidData()
    {
        var dto = new CreateStudentDto("Jane", "Smith", "jane@test.com", "STU-002",
            DateTime.UtcNow.AddYears(-22), null);

        _studentRepo.Setup(r => r.EmailExistsAsync(dto.Email, null, default)).ReturnsAsync(false);
        _studentRepo.Setup(r => r.CodeExistsAsync(dto.StudentCode, null, default)).ReturnsAsync(false);

        var result = await _service.CreateAsync(dto);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Email.Should().Be("jane@test.com");
        _studentRepo.Verify(r => r.AddAsync(It.IsAny<Student>(), default), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [TestMethod]
    public async Task CreateAsync_ShouldFail_WhenEmailAlreadyExists()
    {
        var dto = new CreateStudentDto("Jane", "Smith", "jane@test.com", "STU-002",
            DateTime.UtcNow.AddYears(-22), null);

        _studentRepo.Setup(r => r.EmailExistsAsync(dto.Email, null, default)).ReturnsAsync(true);

        var result = await _service.CreateAsync(dto);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("CONFLICT");
        _studentRepo.Verify(r => r.AddAsync(It.IsAny<Student>(), default), Times.Never);
    }

    [TestMethod]
    public async Task CreateAsync_ShouldFail_WhenCodeAlreadyExists()
    {
        var dto = new CreateStudentDto("Jane", "Smith", "jane2@test.com", "STU-001",
            DateTime.UtcNow.AddYears(-22), null);

        _studentRepo.Setup(r => r.EmailExistsAsync(dto.Email, null, default)).ReturnsAsync(false);
        _studentRepo.Setup(r => r.CodeExistsAsync(dto.StudentCode, null, default)).ReturnsAsync(true);

        var result = await _service.CreateAsync(dto);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("CONFLICT");
    }

    [TestMethod]
    public async Task DeleteAsync_ShouldFail_WhenStudentNotFound()
    {
        _studentRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync((Student?)null);

        var result = await _service.DeleteAsync(Guid.NewGuid());

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("NOT_FOUND");
    }

    [TestMethod]
    public async Task DeleteAsync_ShouldSucceed_WhenStudentExists()
    {
        var student = Student.Create("Test", "User", "t@test.com", "STU-003", DateTime.UtcNow.AddYears(-20));
        _studentRepo.Setup(r => r.GetByIdAsync(student.Id, default)).ReturnsAsync(student);

        var result = await _service.DeleteAsync(student.Id);

        result.IsSuccess.Should().BeTrue();
        student.IsDeleted.Should().BeTrue();
    }
}
