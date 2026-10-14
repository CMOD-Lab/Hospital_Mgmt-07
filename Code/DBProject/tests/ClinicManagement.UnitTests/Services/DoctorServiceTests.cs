using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClinicManagement.UnitTests.Services;

/// <summary>
/// Unit tests for DoctorService.
/// </summary>
public class DoctorServiceTests
{
    private readonly Mock<IDoctorRepository> _doctorRepositoryMock;
    private readonly Mock<IDepartmentRepository> _departmentRepositoryMock;
    private readonly Mock<ILoginRepository> _loginRepositoryMock;
    private readonly Mock<ILogger<DoctorService>> _loggerMock;
    private readonly DoctorService _doctorService;

    public DoctorServiceTests()
    {
        _doctorRepositoryMock = new Mock<IDoctorRepository>();
        _departmentRepositoryMock = new Mock<IDepartmentRepository>();
        _loginRepositoryMock = new Mock<ILoginRepository>();
        _loggerMock = new Mock<ILogger<DoctorService>>();
        _doctorService = new DoctorService(
            _doctorRepositoryMock.Object,
            _departmentRepositoryMock.Object,
            _loginRepositoryMock.Object,
            _loggerMock.Object);
    }

    // ─── GetDoctorByIdAsync ───────────────────────────────────────────────────

    [Fact]
    public async Task GetDoctorByIdAsync_WhenDoctorExists_ReturnsDoctorDto()
    {
        // Arrange
        var department = new Department { DeptNo = 1, DeptName = "Cardiology" };
        var doctor = new Doctor
        {
            DoctorId = 1,
            Name = "Dr. Smith",
            Phone = "9876543210",
            Gender = 'M',
            BirthDate = new DateTime(1975, 6, 15),
            ChargesPerVisit = 500.0,
            ReputeIndex = 4.5,
            PatientsTreated = 100,
            Qualification = "MBBS",
            Specialization = "Cardiology",
            WorkExperience = 15,
            Status = 1,
            Department = department
        };

        _doctorRepositoryMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(doctor);

        // Act
        var result = await _doctorService.GetDoctorByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.DoctorId.Should().Be(1);
        result.Name.Should().Be("Dr. Smith");
        result.Gender.Should().Be("M");
        result.ChargesPerVisit.Should().Be(500.0);
        result.ReputeIndex.Should().Be(4.5);
        result.PatientsTreated.Should().Be(100);
        result.Qualification.Should().Be("MBBS");
        result.Specialization.Should().Be("Cardiology");
        result.WorkExperience.Should().Be(15);
        result.Department.Should().Be("Cardiology");
    }

    [Fact]
    public async Task GetDoctorByIdAsync_WhenDoctorNotFound_ReturnsNull()
    {
        // Arrange
        _doctorRepositoryMock
            .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Doctor?)null);

        // Act
        var result = await _doctorService.GetDoctorByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetDoctorByIdAsync_WhenDoctorHasNoDepartment_ReturnsDoctorDtoWithNullDepartment()
    {
        // Arrange
        var doctor = new Doctor
        {
            DoctorId = 2,
            Name = "Dr. Jones",
            BirthDate = new DateTime(1980, 3, 20),
            Gender = 'F',
            ChargesPerVisit = 300.0,
            Qualification = "MD",
            Status = 1,
            Department = null
        };

        _doctorRepositoryMock
            .Setup(r => r.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(doctor);

        // Act
        var result = await _doctorService.GetDoctorByIdAsync(2);

        // Assert
        result.Should().NotBeNull();
        result!.Department.Should().BeNull();
    }

    [Fact]
    public async Task GetDoctorByIdAsync_WhenRepositoryThrows_ReturnsNull()
    {
        // Arrange
        _doctorRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _doctorService.GetDoctorByIdAsync(1);

        // Assert
        result.Should().BeNull();
    }

    // ─── GetDoctorsAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task GetDoctorsAsync_WithEmptyQuery_ReturnsAllActiveDoctors()
    {
        // Arrange
        var doctors = new List<Doctor>
        {
            new Doctor { DoctorId = 1, Name = "Dr. A", Status = 1, Department = new Department { DeptName = "ENT" } },
            new Doctor { DoctorId = 2, Name = "Dr. B", Status = 1, Department = new Department { DeptName = "Ortho" } }
        };

        _doctorRepositoryMock
            .Setup(r => r.GetActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(doctors);

        // Act
        var result = await _doctorService.GetDoctorsAsync();

        // Assert
        result.Should().HaveCount(2);
        result.First().Name.Should().Be("Dr. A");
    }

    [Fact]
    public async Task GetDoctorsAsync_WithSearchQuery_ReturnsFilteredDoctors()
    {
        // Arrange
        var doctors = new List<Doctor>
        {
            new Doctor { DoctorId = 1, Name = "Dr. Smith", Status = 1, Department = new Department { DeptName = "Cardiology" } }
        };

        _doctorRepositoryMock
            .Setup(r => r.SearchAsync("Smith", It.IsAny<CancellationToken>()))
            .ReturnsAsync(doctors);

        // Act
        var result = await _doctorService.GetDoctorsAsync("Smith");

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Dr. Smith");
    }

    [Fact]
    public async Task GetDoctorsAsync_WhenRepositoryThrows_ReturnsEmptyList()
    {
        // Arrange
        _doctorRepositoryMock
            .Setup(r => r.GetActiveAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _doctorService.GetDoctorsAsync();

        // Assert
        result.Should().BeEmpty();
    }

    // ─── GetDoctorsByDepartmentAsync ──────────────────────────────────────────

    [Fact]
    public async Task GetDoctorsByDepartmentAsync_WithValidDept_ReturnsDoctors()
    {
        // Arrange
        var doctors = new List<Doctor>
        {
            new Doctor { DoctorId = 1, Name = "Dr. Heart", Status = 1, Department = new Department { DeptName = "Cardiology" } }
        };

        _doctorRepositoryMock
            .Setup(r => r.GetByDepartmentAsync("Cardiology", It.IsAny<CancellationToken>()))
            .ReturnsAsync(doctors);

        // Act
        var result = await _doctorService.GetDoctorsByDepartmentAsync("Cardiology");

        // Assert
        result.Should().HaveCount(1);
        result.First().Department.Should().Be("Cardiology");
    }

    [Fact]
    public async Task GetDoctorsByDepartmentAsync_WhenRepositoryThrows_ReturnsEmptyList()
    {
        // Arrange
        _doctorRepositoryMock
            .Setup(r => r.GetByDepartmentAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _doctorService.GetDoctorsByDepartmentAsync("Cardiology");

        // Assert
        result.Should().BeEmpty();
    }

    // ─── AddDoctorAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task AddDoctorAsync_WithNewEmail_ReturnsTrue()
    {
        // Arrange
        var dto = new AddDoctorDto
        {
            Name = "Dr. New",
            Email = "new@clinic.com",
            Password = "pass123",
            BirthDate = "1985-05-10",
            DeptNo = 1,
            Phone = "9876543210",
            Gender = 'M',
            Address = "456 Oak Ave",
            WorkExperience = 10,
            Salary = 80000,
            ChargesPerVisit = 400,
            Specialization = "Neurology",
            Qualification = "MBBS, MD"
        };

        var createdLogin = new LoginTable { LoginId = 10, Email = dto.Email, Password = dto.Password, Type = 2 };

        _loginRepositoryMock
            .Setup(r => r.EmailExistsAsync(dto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _loginRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<LoginTable>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdLogin);

        _doctorRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Doctor>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Doctor { DoctorId = 10 });

        // Act
        var result = await _doctorService.AddDoctorAsync(dto);

        // Assert
        result.Should().BeTrue();
        _doctorRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Doctor>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddDoctorAsync_WithExistingEmail_ReturnsFalse()
    {
        // Arrange
        var dto = new AddDoctorDto
        {
            Name = "Dr. Dup",
            Email = "existing@clinic.com",
            Password = "pass",
            BirthDate = "1980-01-01",
            Gender = 'F'
        };

        _loginRepositoryMock
            .Setup(r => r.EmailExistsAsync(dto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _doctorService.AddDoctorAsync(dto);

        // Assert
        result.Should().BeFalse();
        _doctorRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Doctor>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AddDoctorAsync_WhenRepositoryThrows_ReturnsFalse()
    {
        // Arrange
        var dto = new AddDoctorDto
        {
            Name = "Dr. Error",
            Email = "error@clinic.com",
            Password = "pass",
            BirthDate = "1980-01-01",
            Gender = 'M'
        };

        _loginRepositoryMock
            .Setup(r => r.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _doctorService.AddDoctorAsync(dto);

        // Assert
        result.Should().BeFalse();
    }

    // ─── DeleteDoctorAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteDoctorAsync_WithValidId_ReturnsTrue()
    {
        // Arrange
        _doctorRepositoryMock
            .Setup(r => r.SoftDeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _doctorService.DeleteDoctorAsync(1);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteDoctorAsync_WhenDoctorNotFound_ReturnsFalse()
    {
        // Arrange
        _doctorRepositoryMock
            .Setup(r => r.SoftDeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _doctorService.DeleteDoctorAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteDoctorAsync_WhenRepositoryThrows_ReturnsFalse()
    {
        // Arrange
        _doctorRepositoryMock
            .Setup(r => r.SoftDeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _doctorService.DeleteDoctorAsync(1);

        // Assert
        result.Should().BeFalse();
    }

    // ─── Age calculation edge cases ───────────────────────────────────────────

    [Fact]
    public async Task GetDoctorByIdAsync_AgeCalculation_IsCorrect()
    {
        // Arrange
        var birthDate = DateTime.Today.AddYears(-30);
        var doctor = new Doctor
        {
            DoctorId = 5,
            Name = "Dr. Age",
            BirthDate = birthDate,
            Gender = 'M',
            ChargesPerVisit = 200,
            Qualification = "MBBS",
            Status = 1
        };

        _doctorRepositoryMock
            .Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(doctor);

        // Act
        var result = await _doctorService.GetDoctorByIdAsync(5);

        // Assert
        result.Should().NotBeNull();
        result!.Age.Should().Be(30);
    }
}
