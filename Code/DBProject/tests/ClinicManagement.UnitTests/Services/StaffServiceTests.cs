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
/// Unit tests for StaffService.
/// </summary>
public class StaffServiceTests
{
    private readonly Mock<IStaffRepository> _staffRepositoryMock;
    private readonly Mock<ILogger<StaffService>> _loggerMock;
    private readonly StaffService _staffService;

    public StaffServiceTests()
    {
        _staffRepositoryMock = new Mock<IStaffRepository>();
        _loggerMock = new Mock<ILogger<StaffService>>();
        _staffService = new StaffService(_staffRepositoryMock.Object, _loggerMock.Object);
    }

    private static OtherStaff CreateStaff(int id = 1) => new OtherStaff
    {
        StaffId = id,
        Name = "Jane Doe",
        Phone = "9876543210",
        Address = "789 Pine St",
        Designation = "Nurse",
        Gender = 'F',
        BirthDate = new DateTime(1990, 4, 20),
        HighestQualification = "BSc Nursing",
        Salary = 40000.0
    };

    // ─── GetStaffByIdAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task GetStaffByIdAsync_WhenStaffExists_ReturnsStaffDto()
    {
        // Arrange
        var staff = CreateStaff(1);
        _staffRepositoryMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(staff);

        // Act
        var result = await _staffService.GetStaffByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.StaffId.Should().Be(1);
        result.Name.Should().Be("Jane Doe");
        result.Phone.Should().Be("9876543210");
        result.Address.Should().Be("789 Pine St");
        result.Gender.Should().Be("F");
        result.Designation.Should().Be("Nurse");
        result.Salary.Should().Be(40000.0);
    }

    [Fact]
    public async Task GetStaffByIdAsync_WhenStaffNotFound_ReturnsNull()
    {
        // Arrange
        _staffRepositoryMock
            .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OtherStaff?)null);

        // Act
        var result = await _staffService.GetStaffByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetStaffByIdAsync_WhenRepositoryThrows_ReturnsNull()
    {
        // Arrange
        _staffRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _staffService.GetStaffByIdAsync(1);

        // Assert
        result.Should().BeNull();
    }

    // ─── GetStaffAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task GetStaffAsync_WithEmptyQuery_ReturnsAllStaff()
    {
        // Arrange
        var staffList = new List<OtherStaff>
        {
            CreateStaff(1),
            new OtherStaff { StaffId = 2, Name = "Bob Smith", Designation = "Receptionist", Gender = 'M' }
        };

        _staffRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(staffList);

        // Act
        var result = await _staffService.GetStaffAsync();

        // Assert
        result.Should().HaveCount(2);
        result.First().Name.Should().Be("Jane Doe");
        result.First().Designation.Should().Be("Nurse");
    }

    [Fact]
    public async Task GetStaffAsync_WithSearchQuery_ReturnsFilteredStaff()
    {
        // Arrange
        var staffList = new List<OtherStaff> { CreateStaff(1) };
        _staffRepositoryMock
            .Setup(r => r.SearchAsync("Jane", It.IsAny<CancellationToken>()))
            .ReturnsAsync(staffList);

        // Act
        var result = await _staffService.GetStaffAsync("Jane");

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Jane Doe");
    }

    [Fact]
    public async Task GetStaffAsync_WithWhitespaceQuery_ReturnsAllStaff()
    {
        // Arrange
        var staffList = new List<OtherStaff> { CreateStaff(1) };
        _staffRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(staffList);

        // Act
        var result = await _staffService.GetStaffAsync("   ");

        // Assert
        result.Should().HaveCount(1);
        _staffRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        _staffRepositoryMock.Verify(r => r.SearchAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetStaffAsync_WhenRepositoryThrows_ReturnsEmpty()
    {
        // Arrange
        _staffRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _staffService.GetStaffAsync();

        // Assert
        result.Should().BeEmpty();
    }

    // ─── AddStaffAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task AddStaffAsync_WithValidDto_ReturnsTrue()
    {
        // Arrange
        var dto = new AddStaffDto
        {
            Name = "New Staff",
            BirthDate = "1995-08-15",
            Phone = "1234567890",
            Gender = 'M',
            Address = "100 Elm St",
            Salary = 35000,
            Qualification = "Diploma",
            Designation = "Lab Technician"
        };

        _staffRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<OtherStaff>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OtherStaff { StaffId = 5 });

        // Act
        var result = await _staffService.AddStaffAsync(dto);

        // Assert
        result.Should().BeTrue();
        _staffRepositoryMock.Verify(r => r.AddAsync(
            It.Is<OtherStaff>(s =>
                s.Name == "New Staff" &&
                s.Designation == "Lab Technician" &&
                s.Gender == 'M'),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddStaffAsync_WithEmptyBirthDate_SetsNullBirthDate()
    {
        // Arrange
        var dto = new AddStaffDto
        {
            Name = "No DOB Staff",
            BirthDate = "",
            Phone = "1234567890",
            Gender = 'F',
            Address = "200 Maple Ave",
            Salary = 30000,
            Qualification = "BSc",
            Designation = "Pharmacist"
        };

        OtherStaff? capturedStaff = null;
        _staffRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<OtherStaff>(), It.IsAny<CancellationToken>()))
            .Callback<OtherStaff, CancellationToken>((s, _) => capturedStaff = s)
            .ReturnsAsync(new OtherStaff { StaffId = 6 });

        // Act
        var result = await _staffService.AddStaffAsync(dto);

        // Assert
        result.Should().BeTrue();
        capturedStaff.Should().NotBeNull();
        capturedStaff!.BirthDate.Should().BeNull();
    }

    [Fact]
    public async Task AddStaffAsync_WhenRepositoryThrows_ReturnsFalse()
    {
        // Arrange
        var dto = new AddStaffDto
        {
            Name = "Error Staff",
            BirthDate = "1990-01-01",
            Phone = "1234567890",
            Gender = 'M',
            Designation = "Nurse"
        };

        _staffRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<OtherStaff>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _staffService.AddStaffAsync(dto);

        // Assert
        result.Should().BeFalse();
    }

    // ─── DeleteStaffAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteStaffAsync_WithValidId_ReturnsTrue()
    {
        // Arrange
        _staffRepositoryMock
            .Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _staffService.DeleteStaffAsync(1);

        // Assert
        result.Should().BeTrue();
        _staffRepositoryMock.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteStaffAsync_WhenRepositoryThrows_ReturnsFalse()
    {
        // Arrange
        _staffRepositoryMock
            .Setup(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _staffService.DeleteStaffAsync(1);

        // Assert
        result.Should().BeFalse();
    }
}
