using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Web.Pages.Doctor;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClinicManagement.Web.Tests.Pages.Doctor;

public class HistoryUpdateModelTests
{
    private readonly Mock<DoctorService> _mockDoctorService;

    public HistoryUpdateModelTests()
    {
        var doctorRepoMock = new Mock<IDoctorRepository>();
        var appointmentRepoMock = new Mock<IAppointmentRepository>();
        var loggerMock = new Mock<ILogger<DoctorService>>();
        _mockDoctorService = new Mock<DoctorService>(
            doctorRepoMock.Object, appointmentRepoMock.Object, loggerMock.Object);
    }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        // Arrange & Act
        var model = new HistoryUpdateModel(_mockDoctorService.Object);

        // Assert
        Assert.NotNull(model);
    }

    [Fact]
    public void CurrentAppointment_DefaultValue_IsNull()
    {
        // Arrange
        var model = new HistoryUpdateModel(_mockDoctorService.Object);

        // Assert
        Assert.Null(model.CurrentAppointment);
    }

    [Fact]
    public void Message_DefaultValue_IsNull()
    {
        // Arrange
        var model = new HistoryUpdateModel(_mockDoctorService.Object);

        // Assert
        Assert.Null(model.Message);
    }

    [Fact]
    public async Task OnGetAsync_WhenUserNotLoggedIn_RedirectsToIndex()
    {
        // Arrange
        var model = new HistoryUpdateModel(_mockDoctorService.Object);
        var httpContext = CreateHttpContext();
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnGetAsync(1, CancellationToken.None);

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Index", redirectResult.PageName);
    }

    [Fact]
    public async Task OnGetAsync_WhenUserLoggedIn_ReturnsPageWithCurrentAppointment()
    {
        // Arrange
        var appointments = new List<AppointmentDto>
        {
            new AppointmentDto { AppointId = 1, PatientName = "John Doe", Disease = "Flu" },
            new AppointmentDto { AppointId = 2, PatientName = "Jane Doe" }
        };
        _mockDoctorService.Setup(s => s.GetTodaysAppointmentsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointments);

        var model = new HistoryUpdateModel(_mockDoctorService.Object);
        var httpContext = CreateHttpContext(userId: 1);
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnGetAsync(1, CancellationToken.None);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.NotNull(model.CurrentAppointment);
        Assert.Equal("John Doe", model.CurrentAppointment!.PatientName);
        Assert.Equal(1, model.AppointmentId);
    }

    [Fact]
    public async Task OnGetAsync_WhenAppointmentNotFound_ReturnsPageWithNullAppointment()
    {
        // Arrange
        _mockDoctorService.Setup(s => s.GetTodaysAppointmentsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AppointmentDto>());

        var model = new HistoryUpdateModel(_mockDoctorService.Object);
        var httpContext = CreateHttpContext(userId: 1);
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnGetAsync(999, CancellationToken.None);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Null(model.CurrentAppointment);
    }

    [Fact]
    public async Task OnPostAsync_WhenUserNotLoggedIn_RedirectsToIndex()
    {
        // Arrange
        var model = new HistoryUpdateModel(_mockDoctorService.Object);
        var httpContext = CreateHttpContext();
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnPostAsync(1, "Flu", "Improving", "Paracetamol", CancellationToken.None);

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Index", redirectResult.PageName);
    }

    [Fact]
    public async Task OnPostAsync_WhenUserLoggedIn_UpdatesPrescriptionAndReturnsPage()
    {
        // Arrange
        var appointments = new List<AppointmentDto>
        {
            new AppointmentDto { AppointId = 1, PatientName = "John Doe" }
        };
        _mockDoctorService.Setup(s => s.UpdatePrescriptionAsync(It.IsAny<UpdatePrescriptionDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockDoctorService.Setup(s => s.GetTodaysAppointmentsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointments);

        var model = new HistoryUpdateModel(_mockDoctorService.Object);
        var httpContext = CreateHttpContext(userId: 1);
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnPostAsync(1, "Flu", "Improving", "Paracetamol", CancellationToken.None);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Equal("Prescription updated successfully!", model.Message);
    }

    [Fact]
    public async Task OnPostAsync_CallsUpdatePrescriptionWithCorrectData()
    {
        // Arrange
        UpdatePrescriptionDto? capturedDto = null;
        _mockDoctorService.Setup(s => s.UpdatePrescriptionAsync(It.IsAny<UpdatePrescriptionDto>(), It.IsAny<CancellationToken>()))
            .Callback<UpdatePrescriptionDto, CancellationToken>((dto, _) => capturedDto = dto)
            .ReturnsAsync(true);
        _mockDoctorService.Setup(s => s.GetTodaysAppointmentsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AppointmentDto>());

        var model = new HistoryUpdateModel(_mockDoctorService.Object);
        var httpContext = CreateHttpContext(userId: 1);
        model.PageContext = CreatePageContext(httpContext);

        // Act
        await model.OnPostAsync(5, "Diabetes", "Stable", "Metformin", CancellationToken.None);

        // Assert
        Assert.NotNull(capturedDto);
        Assert.Equal(1, capturedDto!.DoctorId);
        Assert.Equal(5, capturedDto.AppointmentId);
        Assert.Equal("Diabetes", capturedDto.Disease);
        Assert.Equal("Stable", capturedDto.Progress);
        Assert.Equal("Metformin", capturedDto.Prescription);
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
