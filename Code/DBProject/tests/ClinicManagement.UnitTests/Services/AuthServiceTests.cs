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
/// Comprehensive unit tests for AuthService.
/// </summary>
public class AuthServiceTests
{
    private readonly Mock<ILoginRepository> _loginRepositoryMock;
    private readonly Mock<IPatientRepository> _patientRepositoryMock;
    private readonly Mock<ILogger<AuthService>> _loggerMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _loginRepositoryMock = new Mock<ILoginRepository>();
        _patientRepositoryMock = new Mock<IPatientRepository>();
        _loggerMock = new Mock<ILogger<AuthService>>();
        _authService = new AuthService(
            _loginRepositoryMock.Object,
            _patientRepositoryMock.Object,
            _loggerMock.Object);
    }

    // ─── ValidateLoginAsync ───────────────────────────────────────────────────

    [Fact]
    public async Task ValidateLoginAsync_WithValidCredentials_ReturnsSuccess()
    {
        // Arrange
        var login = new LoginTable { LoginId = 1, Email = "test@test.com", Password = "pass123", Type = 1 };
        _loginRepositoryMock
            .Setup(r => r.ValidateLoginAsync("test@test.com", "pass123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(login);

        var request = new LoginRequestDto { Email = "test@test.com", Password = "pass123" };

        // Act
        var result = await _authService.ValidateLoginAsync(request);

        // Assert
        result.Success.Should().BeTrue();
        result.UserId.Should().Be(1);
        result.UserType.Should().Be(1);
        result.Message.Should().Be("Login successful.");
    }

    [Fact]
    public async Task ValidateLoginAsync_WithInvalidCredentials_ReturnsFailure()
    {
        // Arrange
        _loginRepositoryMock
            .Setup(r => r.ValidateLoginAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((LoginTable?)null);

        var request = new LoginRequestDto { Email = "wrong@test.com", Password = "wrongpass" };

        // Act
        var result = await _authService.ValidateLoginAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Invalid email or password");
    }

    [Fact]
    public async Task ValidateLoginAsync_ForDoctorUser_ReturnsCorrectUserType()
    {
        // Arrange
        var login = new LoginTable { LoginId = 5, Email = "doctor@clinic.com", Password = "docpass", Type = 2 };
        _loginRepositoryMock
            .Setup(r => r.ValidateLoginAsync("doctor@clinic.com", "docpass", It.IsAny<CancellationToken>()))
            .ReturnsAsync(login);

        var request = new LoginRequestDto { Email = "doctor@clinic.com", Password = "docpass" };

        // Act
        var result = await _authService.ValidateLoginAsync(request);

        // Assert
        result.Success.Should().BeTrue();
        result.UserId.Should().Be(5);
        result.UserType.Should().Be(2);
    }

    [Fact]
    public async Task ValidateLoginAsync_ForAdminUser_ReturnsCorrectUserType()
    {
        // Arrange
        var login = new LoginTable { LoginId = 99, Email = "admin@clinic.com", Password = "adminpass", Type = 3 };
        _loginRepositoryMock
            .Setup(r => r.ValidateLoginAsync("admin@clinic.com", "adminpass", It.IsAny<CancellationToken>()))
            .ReturnsAsync(login);

        var request = new LoginRequestDto { Email = "admin@clinic.com", Password = "adminpass" };

        // Act
        var result = await _authService.ValidateLoginAsync(request);

        // Assert
        result.Success.Should().BeTrue();
        result.UserType.Should().Be(3);
    }

    [Fact]
    public async Task ValidateLoginAsync_WhenRepositoryThrows_ReturnsFailure()
    {
        // Arrange
        _loginRepositoryMock
            .Setup(r => r.ValidateLoginAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB connection error"));

        var request = new LoginRequestDto { Email = "test@test.com", Password = "pass" };

        // Act
        var result = await _authService.ValidateLoginAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("error occurred");
    }

    // ─── RegisterPatientAsync ─────────────────────────────────────────────────

    [Fact]
    public async Task RegisterPatientAsync_WithExistingEmail_ReturnsFailure()
    {
        // Arrange
        _loginRepositoryMock
            .Setup(r => r.EmailExistsAsync("existing@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var dto = new PatientSignupDto
        {
            Name = "Test",
            Email = "existing@test.com",
            Password = "pass",
            BirthDate = "1990-01-01",
            Gender = "M"
        };

        // Act
        var result = await _authService.RegisterPatientAsync(dto);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Email already exists");
    }

    [Fact]
    public async Task RegisterPatientAsync_WithNewEmail_CreatesLoginAndPatient()
    {
        // Arrange
        var createdLogin = new LoginTable { LoginId = 10, Email = "new@test.com", Password = "pass", Type = 1 };

        _loginRepositoryMock
            .Setup(r => r.EmailExistsAsync("new@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _loginRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<LoginTable>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdLogin);

        _patientRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Patient { PatientId = 10 });

        var dto = new PatientSignupDto
        {
            Name = "New Patient",
            Email = "new@test.com",
            Password = "pass",
            BirthDate = "1990-01-01",
            PhoneNo = "1234567890",
            Gender = "F",
            Address = "123 Main St"
        };

        // Act
        var result = await _authService.RegisterPatientAsync(dto);

        // Assert
        result.Success.Should().BeTrue();
        result.UserId.Should().Be(10);
        result.UserType.Should().Be(1);
        result.Message.Should().Contain("Registration successful");
    }

    [Fact]
    public async Task RegisterPatientAsync_WithNewEmail_CreatesLoginWithCorrectType()
    {
        // Arrange
        var createdLogin = new LoginTable { LoginId = 11, Email = "patient@test.com", Password = "pass", Type = 1 };

        _loginRepositoryMock
            .Setup(r => r.EmailExistsAsync("patient@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _loginRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<LoginTable>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdLogin);

        _patientRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Patient { PatientId = 11 });

        var dto = new PatientSignupDto
        {
            Name = "Patient",
            Email = "patient@test.com",
            Password = "pass",
            BirthDate = "1985-06-15",
            Gender = "M"
        };

        // Act
        var result = await _authService.RegisterPatientAsync(dto);

        // Assert
        _loginRepositoryMock.Verify(r => r.AddAsync(
            It.Is<LoginTable>(l => l.Type == 1 && l.Email == "patient@test.com"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegisterPatientAsync_WithEmptyGender_DefaultsToMale()
    {
        // Arrange
        var createdLogin = new LoginTable { LoginId = 12, Email = "nogender@test.com", Password = "pass", Type = 1 };

        _loginRepositoryMock
            .Setup(r => r.EmailExistsAsync("nogender@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _loginRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<LoginTable>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdLogin);

        Patient? capturedPatient = null;
        _patientRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>()))
            .Callback<Patient, CancellationToken>((p, _) => capturedPatient = p)
            .ReturnsAsync(new Patient { PatientId = 12 });

        var dto = new PatientSignupDto
        {
            Name = "No Gender",
            Email = "nogender@test.com",
            Password = "pass",
            BirthDate = "1990-01-01",
            Gender = "" // Empty gender
        };

        // Act
        var result = await _authService.RegisterPatientAsync(dto);

        // Assert
        result.Success.Should().BeTrue();
        capturedPatient.Should().NotBeNull();
        capturedPatient!.Gender.Should().Be('M');
    }

    [Fact]
    public async Task RegisterPatientAsync_WhenRepositoryThrows_ReturnsFailure()
    {
        // Arrange
        _loginRepositoryMock
            .Setup(r => r.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        var dto = new PatientSignupDto
        {
            Name = "Error Patient",
            Email = "error@test.com",
            Password = "pass",
            BirthDate = "1990-01-01",
            Gender = "M"
        };

        // Act
        var result = await _authService.RegisterPatientAsync(dto);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("error occurred");
    }

    [Fact]
    public async Task RegisterPatientAsync_WithFemaleGender_SetsCorrectGender()
    {
        // Arrange
        var createdLogin = new LoginTable { LoginId = 13, Email = "female@test.com", Password = "pass", Type = 1 };

        _loginRepositoryMock
            .Setup(r => r.EmailExistsAsync("female@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _loginRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<LoginTable>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdLogin);

        Patient? capturedPatient = null;
        _patientRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>()))
            .Callback<Patient, CancellationToken>((p, _) => capturedPatient = p)
            .ReturnsAsync(new Patient { PatientId = 13 });

        var dto = new PatientSignupDto
        {
            Name = "Female Patient",
            Email = "female@test.com",
            Password = "pass",
            BirthDate = "1992-08-25",
            Gender = "F"
        };

        // Act
        var result = await _authService.RegisterPatientAsync(dto);

        // Assert
        capturedPatient.Should().NotBeNull();
        capturedPatient!.Gender.Should().Be('F');
    }

    [Fact]
    public async Task RegisterPatientAsync_SetsCorrectPatientDetails()
    {
        // Arrange
        var createdLogin = new LoginTable { LoginId = 14, Email = "details@test.com", Password = "pass", Type = 1 };

        _loginRepositoryMock
            .Setup(r => r.EmailExistsAsync("details@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _loginRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<LoginTable>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdLogin);

        Patient? capturedPatient = null;
        _patientRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>()))
            .Callback<Patient, CancellationToken>((p, _) => capturedPatient = p)
            .ReturnsAsync(new Patient { PatientId = 14 });

        var dto = new PatientSignupDto
        {
            Name = "Detail Patient",
            Email = "details@test.com",
            Password = "pass",
            BirthDate = "1988-12-10",
            PhoneNo = "9876543210",
            Gender = "M",
            Address = "456 Oak Ave"
        };

        // Act
        await _authService.RegisterPatientAsync(dto);

        // Assert
        capturedPatient.Should().NotBeNull();
        capturedPatient!.Name.Should().Be("Detail Patient");
        capturedPatient.Phone.Should().Be("9876543210");
        capturedPatient.Address.Should().Be("456 Oak Ave");
        capturedPatient.BirthDate.Should().Be(new DateTime(1988, 12, 10));
    }
}
