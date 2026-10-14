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
/// Comprehensive unit tests for PatientService.
/// </summary>
public class PatientServiceTests
{
    private readonly Mock<IPatientRepository> _patientRepositoryMock;
    private readonly Mock<ILogger<PatientService>> _loggerMock;
    private readonly PatientService _patientService;

    public PatientServiceTests()
    {
        _patientRepositoryMock = new Mock<IPatientRepository>();
        _loggerMock = new Mock<ILogger<PatientService>>();
        _patientService = new PatientService(_patientRepositoryMock.Object, _loggerMock.Object);
    }

    // ─── GetPatientByIdAsync ──────────────────────────────────────────────────

    [Fact]
    public async Task GetPatientByIdAsync_WhenPatientExists_ReturnsPatientDto()
    {
        // Arrange
        var patient = new Patient
        {
            PatientId = 1,
            Name = "John Doe",
            Phone = "12345678901",
            Address = "123 Main St",
            BirthDate = new DateTime(1990, 1, 1),
            Gender = 'M'
        };

        _patientRepositoryMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(patient);

        // Act
        var result = await _patientService.GetPatientByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.PatientId.Should().Be(1);
        result.Name.Should().Be("John Doe");
        result.Gender.Should().Be("M");
        result.Phone.Should().Be("12345678901");
        result.Address.Should().Be("123 Main St");
    }

    [Fact]
    public async Task GetPatientByIdAsync_WhenPatientNotFound_ReturnsNull()
    {
        // Arrange
        _patientRepositoryMock
            .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Patient?)null);

        // Act
        var result = await _patientService.GetPatientByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPatientByIdAsync_WhenRepositoryThrows_ReturnsNull()
    {
        // Arrange
        _patientRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _patientService.GetPatientByIdAsync(1);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPatientByIdAsync_AgeCalculation_IsCorrect()
    {
        // Arrange
        var birthDate = DateTime.Today.AddYears(-25);
        var patient = new Patient
        {
            PatientId = 2,
            Name = "Young Patient",
            BirthDate = birthDate,
            Gender = 'F'
        };

        _patientRepositoryMock
            .Setup(r => r.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(patient);

        // Act
        var result = await _patientService.GetPatientByIdAsync(2);

        // Assert
        result.Should().NotBeNull();
        result!.Age.Should().Be(25);
    }

    [Fact]
    public async Task GetPatientByIdAsync_BirthDateFormat_IsCorrect()
    {
        // Arrange
        var patient = new Patient
        {
            PatientId = 3,
            Name = "Date Patient",
            BirthDate = new DateTime(1985, 11, 20),
            Gender = 'M'
        };

        _patientRepositoryMock
            .Setup(r => r.GetByIdAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(patient);

        // Act
        var result = await _patientService.GetPatientByIdAsync(3);

        // Assert
        result.Should().NotBeNull();
        result!.BirthDate.Should().Be("1985-11-20");
    }

    [Fact]
    public async Task GetPatientByIdAsync_FemalePatient_ReturnsCorrectGender()
    {
        // Arrange
        var patient = new Patient
        {
            PatientId = 4,
            Name = "Jane Doe",
            BirthDate = new DateTime(1992, 5, 10),
            Gender = 'F'
        };

        _patientRepositoryMock
            .Setup(r => r.GetByIdAsync(4, It.IsAny<CancellationToken>()))
            .ReturnsAsync(patient);

        // Act
        var result = await _patientService.GetPatientByIdAsync(4);

        // Assert
        result.Should().NotBeNull();
        result!.Gender.Should().Be("F");
    }

    // ─── GetPatientsAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task GetPatientsAsync_WithEmptyQuery_ReturnsAllPatients()
    {
        // Arrange
        var patients = new List<Patient>
        {
            new Patient { PatientId = 1, Name = "Alice", BirthDate = DateTime.Now.AddYears(-30), Gender = 'F' },
            new Patient { PatientId = 2, Name = "Bob", BirthDate = DateTime.Now.AddYears(-25), Gender = 'M' }
        };

        _patientRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(patients);

        // Act
        var result = await _patientService.GetPatientsAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetPatientsAsync_WithSearchQuery_ReturnsFilteredPatients()
    {
        // Arrange
        var patients = new List<Patient>
        {
            new Patient { PatientId = 1, Name = "Alice Smith", BirthDate = DateTime.Now.AddYears(-30), Gender = 'F' }
        };

        _patientRepositoryMock
            .Setup(r => r.SearchAsync("Alice", It.IsAny<CancellationToken>()))
            .ReturnsAsync(patients);

        // Act
        var result = await _patientService.GetPatientsAsync("Alice");

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Alice Smith");
    }

    [Fact]
    public async Task GetPatientsAsync_WithWhitespaceQuery_ReturnsAllPatients()
    {
        // Arrange
        var patients = new List<Patient>
        {
            new Patient { PatientId = 1, Name = "Test", BirthDate = DateTime.Now.AddYears(-20), Gender = 'M' }
        };

        _patientRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(patients);

        // Act
        var result = await _patientService.GetPatientsAsync("   ");

        // Assert
        result.Should().HaveCount(1);
        _patientRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        _patientRepositoryMock.Verify(r => r.SearchAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetPatientsAsync_MapsToPatientListDto_Correctly()
    {
        // Arrange
        var patients = new List<Patient>
        {
            new Patient { PatientId = 5, Name = "Charlie Brown", Phone = "5551234567", BirthDate = DateTime.Now.AddYears(-40), Gender = 'M' }
        };

        _patientRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(patients);

        // Act
        var result = await _patientService.GetPatientsAsync();

        // Assert
        var dto = result.Single();
        dto.PatientId.Should().Be(5);
        dto.Name.Should().Be("Charlie Brown");
        dto.Phone.Should().Be("5551234567");
    }

    [Fact]
    public async Task GetPatientsAsync_WhenRepositoryThrows_ReturnsEmpty()
    {
        // Arrange
        _patientRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _patientService.GetPatientsAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetPatientsAsync_SearchThrows_ReturnsEmpty()
    {
        // Arrange
        _patientRepositoryMock
            .Setup(r => r.SearchAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _patientService.GetPatientsAsync("query");

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetPatientsAsync_WithEmptyResult_ReturnsEmptyList()
    {
        // Arrange
        _patientRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Patient>());

        // Act
        var result = await _patientService.GetPatientsAsync();

        // Assert
        result.Should().BeEmpty();
    }
}
