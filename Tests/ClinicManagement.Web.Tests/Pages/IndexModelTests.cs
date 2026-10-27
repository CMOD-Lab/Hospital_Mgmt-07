using System;
using System.Threading;
using System.Threading.Tasks;
using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using ClinicManagement.Domain.Enums;
using ClinicManagement.Web.Pages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClinicManagement.Web.Tests.Pages;

public class IndexModelTests
{
    private readonly Mock<AuthService> _mockAuthService;
    private readonly Mock<ILogger<IndexModel>> _mockLogger;

    public IndexModelTests()
    {
        _mockAuthService = new Mock<AuthService>(MockBehavior.Loose,
            new Mock<object>().Object, new Mock<object>().Object, new Mock<object>().Object);
        _mockLogger = new Mock<ILogger<IndexModel>>();
    }

    private static DefaultHttpContext CreateHttpContext(int? userId = null, int? userType = null)
    {
        var context = new DefaultHttpContext();
        var session = new MockSession();
        if (userId.HasValue)
            session.SetInt32("UserId", userId.Value);
        if (userType.HasValue)
            session.SetInt32("UserType", userType.Value);
        context.Session = session;
        return context;
    }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        // Arrange & Act
        var authServiceMock = CreateAuthServiceMock();
        var loggerMock = new Mock<ILogger<IndexModel>>();
        var model = new IndexModel(authServiceMock.Object, loggerMock.Object);

        // Assert
        Assert.NotNull(model);
    }

    [Fact]
    public void OnGet_WhenUserNotLoggedIn_ReturnsPage()
    {
        // Arrange
        var authServiceMock = CreateAuthServiceMock();
        var model = new IndexModel(authServiceMock.Object, _mockLogger.Object);
        var httpContext = CreateHttpContext();
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = model.OnGet();

        // Assert
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public void OnGet_WhenPatientLoggedIn_RedirectsToPatientHome()
    {
        // Arrange
        var authServiceMock = CreateAuthServiceMock();
        var model = new IndexModel(authServiceMock.Object, _mockLogger.Object);
        var httpContext = CreateHttpContext(userId: 1, userType: (int)UserType.Patient);
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = model.OnGet();

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Patient/PatientHome", redirectResult.PageName);
    }

    [Fact]
    public void OnGet_WhenDoctorLoggedIn_RedirectsToDoctorHome()
    {
        // Arrange
        var authServiceMock = CreateAuthServiceMock();
        var model = new IndexModel(authServiceMock.Object, _mockLogger.Object);
        var httpContext = CreateHttpContext(userId: 2, userType: (int)UserType.Doctor);
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = model.OnGet();

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Doctor/DoctorHome", redirectResult.PageName);
    }

    [Fact]
    public void OnGet_WhenAdminLoggedIn_RedirectsToAdminHome()
    {
        // Arrange
        var authServiceMock = CreateAuthServiceMock();
        var model = new IndexModel(authServiceMock.Object, _mockLogger.Object);
        var httpContext = CreateHttpContext(userId: 3, userType: (int)UserType.Admin);
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = model.OnGet();

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Admin/AdminHome", redirectResult.PageName);
    }

    [Fact]
    public async Task OnPostLoginAsync_WithValidCredentials_RedirectsToPatientHome()
    {
        // Arrange
        var authServiceMock = CreateAuthServiceMock();
        authServiceMock.Setup(s => s.ValidateLoginAsync(It.IsAny<LoginRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LoginResultDto { Success = true, UserId = 1, UserType = UserType.Patient });

        var model = new IndexModel(authServiceMock.Object, _mockLogger.Object);
        var httpContext = CreateHttpContext();
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnPostLoginAsync("test@test.com", "password", CancellationToken.None);

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Patient/PatientHome", redirectResult.PageName);
    }

    [Fact]
    public async Task OnPostLoginAsync_WithInvalidCredentials_ReturnsPageWithError()
    {
        // Arrange
        var authServiceMock = CreateAuthServiceMock();
        authServiceMock.Setup(s => s.ValidateLoginAsync(It.IsAny<LoginRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LoginResultDto { Success = false, ErrorMessage = "Invalid credentials" });

        var model = new IndexModel(authServiceMock.Object, _mockLogger.Object);
        var httpContext = CreateHttpContext();
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnPostLoginAsync("test@test.com", "wrongpassword", CancellationToken.None);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Equal("Invalid credentials", model.ErrorMessage);
    }

    [Fact]
    public async Task OnPostLoginAsync_WithDoctorCredentials_RedirectsToDoctorHome()
    {
        // Arrange
        var authServiceMock = CreateAuthServiceMock();
        authServiceMock.Setup(s => s.ValidateLoginAsync(It.IsAny<LoginRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LoginResultDto { Success = true, UserId = 2, UserType = UserType.Doctor });

        var model = new IndexModel(authServiceMock.Object, _mockLogger.Object);
        var httpContext = CreateHttpContext();
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnPostLoginAsync("doctor@test.com", "password", CancellationToken.None);

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Doctor/DoctorHome", redirectResult.PageName);
    }

    [Fact]
    public async Task OnPostLoginAsync_WithAdminCredentials_RedirectsToAdminHome()
    {
        // Arrange
        var authServiceMock = CreateAuthServiceMock();
        authServiceMock.Setup(s => s.ValidateLoginAsync(It.IsAny<LoginRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LoginResultDto { Success = true, UserId = 3, UserType = UserType.Admin });

        var model = new IndexModel(authServiceMock.Object, _mockLogger.Object);
        var httpContext = CreateHttpContext();
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnPostLoginAsync("admin@test.com", "password", CancellationToken.None);

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Admin/AdminHome", redirectResult.PageName);
    }

    [Fact]
    public async Task OnPostSignupAsync_WithValidData_RedirectsToPatientHome()
    {
        // Arrange
        var authServiceMock = CreateAuthServiceMock();
        authServiceMock.Setup(s => s.RegisterPatientAsync(It.IsAny<PatientSignupDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LoginResultDto { Success = true, UserId = 1, UserType = UserType.Patient });

        var model = new IndexModel(authServiceMock.Object, _mockLogger.Object);
        var httpContext = CreateHttpContext();
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnPostSignupAsync(
            "John Doe", "1990-01-01", "john@test.com", "password",
            "1234567890", "M", "123 Main St", CancellationToken.None);

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Patient/PatientHome", redirectResult.PageName);
    }

    [Fact]
    public async Task OnPostSignupAsync_WithExistingEmail_ReturnsPageWithError()
    {
        // Arrange
        var authServiceMock = CreateAuthServiceMock();
        authServiceMock.Setup(s => s.RegisterPatientAsync(It.IsAny<PatientSignupDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LoginResultDto { Success = false, ErrorMessage = "Email already exists." });

        var model = new IndexModel(authServiceMock.Object, _mockLogger.Object);
        var httpContext = CreateHttpContext();
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnPostSignupAsync(
            "John Doe", "1990-01-01", "existing@test.com", "password",
            "1234567890", "M", "123 Main St", CancellationToken.None);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Equal("Email already exists.", model.ErrorMessage);
    }

    [Fact]
    public void ErrorMessage_DefaultValue_IsNull()
    {
        // Arrange
        var authServiceMock = CreateAuthServiceMock();
        var model = new IndexModel(authServiceMock.Object, _mockLogger.Object);

        // Assert
        Assert.Null(model.ErrorMessage);
    }

    private static Mock<AuthService> CreateAuthServiceMock()
    {
        var loginRepoMock = new Mock<ClinicManagement.Domain.Interfaces.Repositories.ILoginRepository>();
        var patientRepoMock = new Mock<ClinicManagement.Domain.Interfaces.Repositories.IPatientRepository>();
        var loggerMock = new Mock<ILogger<AuthService>>();
        return new Mock<AuthService>(loginRepoMock.Object, patientRepoMock.Object, loggerMock.Object);
    }

    private static PageContext CreatePageContext(HttpContext httpContext)
    {
        var actionContext = new ActionContext(
            httpContext,
            new Microsoft.AspNetCore.Routing.RouteData(),
            new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
        return new PageContext(actionContext);
    }
}

/// <summary>Mock session for testing.</summary>
public class MockSession : ISession
{
    private readonly Dictionary<string, byte[]> _store = new();

    public bool IsAvailable => true;
    public string Id => Guid.NewGuid().ToString();
    public IEnumerable<string> Keys => _store.Keys;

    public void Clear() => _store.Clear();
    public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public void Remove(string key) => _store.Remove(key);

    public void Set(string key, byte[] value) => _store[key] = value;

    public bool TryGetValue(string key, out byte[] value)
    {
        if (_store.TryGetValue(key, out var stored))
        {
            value = stored;
            return true;
        }
        value = Array.Empty<byte>();
        return false;
    }
}
