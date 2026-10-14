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
/// Unit tests for PatientService.
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
}
