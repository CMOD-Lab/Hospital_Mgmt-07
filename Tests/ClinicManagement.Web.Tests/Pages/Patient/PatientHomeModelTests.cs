using System;
using System.Threading;
using System.Threading.Tasks;
using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Web.Pages.Patient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClinicManagement.Web.Tests.Pages.Patient;

public class PatientHomeModelTests
{
    private readonly Mock<PatientService> _mockPatientService;
    private readonly Mock<ILogger<PatientHomeModel>> _mockLogger;

    public PatientHomeModelTests()
    {
        var appointmentRepoMock = new Mock<IAppointmentRepository>();
        var patientRepoMock = new Mock<IPatientRepository>();
        var doctorRepoMock = new Mock<IDoctorRepository>();
        var deptRepoMock = new Mock<IDepartmentRepository>();
        var loggerMock = new Mock<ILogger<PatientService>>();
        _mockPatientService = new Mock<PatientService>(
            patientRepoMock.Object, appointmentRepoMock.Object,
            doctorRepoMock.Object, deptRepoMock.Object, loggerMock.Object);
        _mockLogger = new Mock<ILogger<PatientHomeModel>>();
    }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        // Arrange & Act
        var model = new PatientHomeModel(_mockPatientService.Object, _mockLogger.Object);

        // Assert
        Assert.NotNull(model);
    }

    [Fact]
    public void PatientInfo_DefaultValue_IsNull()
    {
        // Arrange
        var model = new PatientHomeModel(_mockPatientService.Object, _mockLogger.Object);

        // Assert
        Assert.Null(model.PatientInfo);
    }

    [Fact]
    public async Task OnGetAsync_WhenUserNotLoggedIn_RedirectsToIndex()
    {
        // Arrange
        var model = new PatientHomeModel(_mockPatientService.Object, _mockLogger.Object);
        var httpContext = CreateHttpContext();
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Index", redirectResult.PageName);
    }

    [Fact]
    public async Task OnGetAsync_WhenUserLoggedIn_ReturnsPageWithPatientInfo()
    {
        // Arrange
        var patientDto = new PatientDto { PatientId = 1, Name = "John Doe" };
        _mockPatientService.Setup(s => s.GetPatientInfoAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(patientDto);

        var model = new PatientHomeModel(_mockPatientService.Object, _mockLogger.Object);
        var httpContext = CreateHttpContext(userId: 1);
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.NotNull(model.PatientInfo);
        Assert.Equal("John Doe", model.PatientInfo!.Name);
    }

    [Fact]
    public async Task OnGetAsync_WhenPatientInfoIsNull_ReturnsPage()
    {
        // Arrange
        _mockPatientService.Setup(s => s.GetPatientInfoAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PatientDto?)null);

        var model = new PatientHomeModel(_mockPatientService.Object, _mockLogger.Object);
        var httpContext = CreateHttpContext(userId: 1);
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Null(model.PatientInfo);
    }

    private static DefaultHttpContext CreateHttpContext(int? userId = null)
    {
        var context = new DefaultHttpContext();
        var session = new MockSession();
        if (userId.HasValue)
            session.SetInt32("UserId", userId.Value);
        context.Session = session;
        return context;
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
