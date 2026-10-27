using System;
using System.Collections.Generic;
using System.Linq;
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

public class AppointmentTakerModelTests
{
    private readonly Mock<PatientService> _mockPatientService;

    public AppointmentTakerModelTests()
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
        var model = new AppointmentTakerModel(_mockPatientService.Object);

        // Assert
        Assert.NotNull(model);
    }

    [Fact]
    public void FreeSlots_DefaultValue_IsEmpty()
    {
        // Arrange
        var model = new AppointmentTakerModel(_mockPatientService.Object);

        // Assert
        Assert.Empty(model.FreeSlots);
    }

    [Fact]
    public void Message_DefaultValue_IsNull()
    {
        // Arrange
        var model = new AppointmentTakerModel(_mockPatientService.Object);

        // Assert
        Assert.Null(model.Message);
    }

    [Fact]
    public async Task OnGetAsync_WhenUserNotLoggedIn_RedirectsToIndex()
    {
        // Arrange
        var model = new AppointmentTakerModel(_mockPatientService.Object);
        var httpContext = CreateHttpContext();
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnGetAsync(1, CancellationToken.None);

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Index", redirectResult.PageName);
    }

    [Fact]
    public async Task OnGetAsync_WhenUserLoggedIn_ReturnsPageWithFreeSlots()
    {
        // Arrange
        var freeSlots = new List<AppointmentDto>
        {
            new AppointmentDto { AppointId = 1, Timings = "09:00 AM" },
            new AppointmentDto { AppointId = 2, Timings = "10:00 AM" }
        };
        _mockPatientService.Setup(s => s.GetFreeSlotsAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(freeSlots);

        var model = new AppointmentTakerModel(_mockPatientService.Object);
        var httpContext = CreateHttpContext(userId: 1);
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnGetAsync(1, CancellationToken.None);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Equal(2, model.FreeSlots.Count());
        Assert.Equal(1, model.DoctorId);
    }

    [Fact]
    public async Task OnPostAsync_WhenUserNotLoggedIn_RedirectsToIndex()
    {
        // Arrange
        var model = new AppointmentTakerModel(_mockPatientService.Object);
        var httpContext = CreateHttpContext();
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnPostAsync(1, 1, CancellationToken.None);

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Index", redirectResult.PageName);
    }

    [Fact]
    public async Task OnPostAsync_WhenBookingSucceeds_RedirectsToAppointmentRequestSent()
    {
        // Arrange
        _mockPatientService.Setup(s => s.BookAppointmentAsync(1, 1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var model = new AppointmentTakerModel(_mockPatientService.Object);
        var httpContext = CreateHttpContext(userId: 1);
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnPostAsync(1, 1, CancellationToken.None);

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Patient/AppointmentRequestSent", redirectResult.PageName);
    }

    [Fact]
    public async Task OnPostAsync_WhenBookingFails_ReturnsPageWithErrorMessage()
    {
        // Arrange
        _mockPatientService.Setup(s => s.BookAppointmentAsync(1, 1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockPatientService.Setup(s => s.GetFreeSlotsAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AppointmentDto>());

        var model = new AppointmentTakerModel(_mockPatientService.Object);
        var httpContext = CreateHttpContext(userId: 1);
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnPostAsync(1, 1, CancellationToken.None);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Equal("Error booking appointment. Please try again.", model.Message);
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
