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

public class PatientNotificationsModelTests
{
    private readonly Mock<PatientService> _mockPatientService;

    public PatientNotificationsModelTests()
    {
        var appointmentRepoMock = new Mock<IAppointmentRepository>();
        var patientRepoMock = new Mock<IPatientRepository>();
        var doctorRepoMock = new Mock<IDoctorRepository>();
        var deptRepoMock = new Mock<IDepartmentRepository>();
        var loggerMock = new Mock<ILogger<PatientService>>();
        _mockPatientService = new Mock<PatientService>(
            patientRepoMock.Object, appointmentRepoMock.Object,
            doctorRepoMock.Object, deptRepoMock.Object, loggerMock.Object);
    }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        // Arrange & Act
        var model = new PatientNotificationsModel(_mockPatientService.Object);

        // Assert
        Assert.NotNull(model);
    }

    [Fact]
    public void Notification_DefaultValue_IsNull()
    {
        // Arrange
        var model = new PatientNotificationsModel(_mockPatientService.Object);

        // Assert
        Assert.Null(model.Notification);
    }

    [Fact]
    public async Task OnGetAsync_WhenUserNotLoggedIn_RedirectsToIndex()
    {
        // Arrange
        var model = new PatientNotificationsModel(_mockPatientService.Object);
        var httpContext = CreateHttpContext();
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Index", redirectResult.PageName);
    }

    [Fact]
    public async Task OnGetAsync_WhenUserLoggedIn_ReturnsPageWithNotification()
    {
        // Arrange
        var notificationDto = new AppointmentDto { AppointId = 1, DoctorName = "Dr. Smith" };
        _mockPatientService.Setup(s => s.GetNotificationAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notificationDto);

        var model = new PatientNotificationsModel(_mockPatientService.Object);
        var httpContext = CreateHttpContext(userId: 1);
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.NotNull(model.Notification);
        Assert.Equal("Dr. Smith", model.Notification!.DoctorName);
    }

    [Fact]
    public async Task OnGetAsync_WhenNoNotification_ReturnsPageWithNullNotification()
    {
        // Arrange
        _mockPatientService.Setup(s => s.GetNotificationAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AppointmentDto?)null);

        var model = new PatientNotificationsModel(_mockPatientService.Object);
        var httpContext = CreateHttpContext(userId: 1);
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Null(model.Notification);
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
