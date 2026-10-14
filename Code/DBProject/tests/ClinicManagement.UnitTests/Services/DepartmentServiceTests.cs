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
/// Unit tests for DepartmentService.
/// </summary>
public class DepartmentServiceTests
{
    private readonly Mock<IDepartmentRepository> _departmentRepositoryMock;
    private readonly Mock<ILogger<DepartmentService>> _loggerMock;
    private readonly DepartmentService _departmentService;

    public DepartmentServiceTests()
    {
        _departmentRepositoryMock = new Mock<IDepartmentRepository>();
        _loggerMock = new Mock<ILogger<DepartmentService>>();
        _departmentService = new DepartmentService(_departmentRepositoryMock.Object, _loggerMock.Object);
    }

    // ─── GetAllDepartmentsAsync ───────────────────────────────────────────────

    [Fact]
    public async Task GetAllDepartmentsAsync_WithDepartments_ReturnsDtos()
    {
        // Arrange
        var departments = new List<Department>
        {
            new Department
            {
                DeptNo = 1,
                DeptName = "Cardiology",
                Description = "Heart care",
                Doctors = new List<Doctor>
                {
                    new Doctor { DoctorId = 1, Status = 1 },
                    new Doctor { DoctorId = 2, Status = 1 }
                }
            },
            new Department
            {
                DeptNo = 2,
                DeptName = "Neurology",
                Description = "Brain care",
                Doctors = new List<Doctor>
                {
                    new Doctor { DoctorId = 3, Status = 1 },
                    new Doctor { DoctorId = 4, Status = 0 } // Left
                }
            }
        };

        _departmentRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(departments);

        // Act
        var result = await _departmentService.GetAllDepartmentsAsync();

        // Assert
        result.Should().HaveCount(2);

        var cardiology = result.First(d => d.DeptName == "Cardiology");
        cardiology.DeptNo.Should().Be(1);
        cardiology.Description.Should().Be("Heart care");
        cardiology.DoctorCount.Should().Be(2); // Both active

        var neurology = result.First(d => d.DeptName == "Neurology");
        neurology.DoctorCount.Should().Be(1); // Only 1 active
    }

    [Fact]
    public async Task GetAllDepartmentsAsync_WithNoDepartments_ReturnsEmptyList()
    {
        // Arrange
        _departmentRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Department>());

        // Act
        var result = await _departmentService.GetAllDepartmentsAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllDepartmentsAsync_WithNullDoctors_DoctorCountIsZero()
    {
        // Arrange
        var departments = new List<Department>
        {
            new Department
            {
                DeptNo = 1,
                DeptName = "Orthopedics",
                Description = "Bone care",
                Doctors = null!
            }
        };

        _departmentRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(departments);

        // Act
        var result = await _departmentService.GetAllDepartmentsAsync();

        // Assert
        result.Should().HaveCount(1);
        result.First().DoctorCount.Should().Be(0);
    }

    [Fact]
    public async Task GetAllDepartmentsAsync_WhenRepositoryThrows_ReturnsEmpty()
    {
        // Arrange
        _departmentRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _departmentService.GetAllDepartmentsAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllDepartmentsAsync_MapsAllFieldsCorrectly()
    {
        // Arrange
        var departments = new List<Department>
        {
            new Department
            {
                DeptNo = 5,
                DeptName = "Dermatology",
                Description = "Skin care",
                Doctors = new List<Doctor>()
            }
        };

        _departmentRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(departments);

        // Act
        var result = await _departmentService.GetAllDepartmentsAsync();

        // Assert
        var dto = result.Single();
        dto.DeptNo.Should().Be(5);
        dto.DeptName.Should().Be("Dermatology");
        dto.Description.Should().Be("Skin care");
        dto.DoctorCount.Should().Be(0);
    }

    [Fact]
    public async Task GetAllDepartmentsAsync_WithCancellationToken_PassesTokenToRepository()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        var token = cts.Token;

        _departmentRepositoryMock
            .Setup(r => r.GetAllAsync(token))
            .ReturnsAsync(new List<Department>());

        // Act
        var result = await _departmentService.GetAllDepartmentsAsync(token);

        // Assert
        _departmentRepositoryMock.Verify(r => r.GetAllAsync(token), Times.Once);
    }
}
