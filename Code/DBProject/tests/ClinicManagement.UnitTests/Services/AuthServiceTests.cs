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
/// Unit tests for AuthService.
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
    }

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
}
