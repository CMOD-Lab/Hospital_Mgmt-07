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

public class BillModelTests
{
    private readonly Mock<DoctorService> _mockDoctorService;

    public BillModelTests()
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
        var model = new BillModel(_mockDoctorService.Object);

        // Assert
        Assert.NotNull(model);
    }

    [Fact]
    public void Appointments_DefaultValue_IsEmpty()
    {
        // Arrange
        var model = new BillModel(_mockDoctorService.Object);

        // Assert
        Assert.Empty(model.Appointments);
    }

    [Fact]
    public void Message_DefaultValue_IsNull()
    {
        // Arrange
        var model = new BillModel(_mockDoctorService.Object);

        // Assert
        Assert.Null(model.Message);
    }

    [Fact]
    public async Task OnGetAsync_WhenUserNotLoggedIn_RedirectsToIndex()
    {
        // Arrange
        var model = new BillModel(_mockDoctorService.Object);
        var httpContext = CreateHttpContext();
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Index", redirectResult.PageName);
    }

    [Fact]
    public async Task OnGetAsync_WhenUserLoggedIn_ReturnsPageWithBillableAppointments()
    {
        // Arrange
        var appointments = new List<AppointmentDto>
        {
            new AppointmentDto { AppointId = 1, BillAmount = 150.0, BillStatus = "Unpaid" },
            new AppointmentDto { AppointId = 2, BillAmount = 200.0, BillStatus = "Paid" }
        };
        _mockDoctorService.Setup(s => s.GetBillableAppointmentsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointments);

        var model = new BillModel(_mockDoctorService.Object);
        var httpContext = CreateHttpContext(userId: 1);
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Equal(2, model.Appointments.Count());
    }

    [Fact]
    public async Task OnPostPaidAsync_WhenUserNotLoggedIn_RedirectsToIndex()
    {
        // Arrange
        var model = new BillModel(_mockDoctorService.Object);
        var httpContext = CreateHttpContext();
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnPostPaidAsync(1, CancellationToken.None);

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Index", redirectResult.PageName);
    }

    [Fact]
    public async Task OnPostPaidAsync_WhenUserLoggedIn_MarksAsPaidAndReturnsPage()
    {
        // Arrange
        _mockDoctorService.Setup(s => s.MarkAsPaidAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockDoctorService.Setup(s => s.GetBillableAppointmentsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AppointmentDto>());

        var model = new BillModel(_mockDoctorService.Object);
        var httpContext = CreateHttpContext(userId: 1);
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnPostPaidAsync(1, CancellationToken.None);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Equal("Appointment marked as paid.", model.Message);
    }

    [Fact]
    public async Task OnPostUnpaidAsync_WhenUserNotLoggedIn_RedirectsToIndex()
    {
        // Arrange
        var model = new BillModel(_mockDoctorService.Object);
        var httpContext = CreateHttpContext();
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnPostUnpaidAsync(1, CancellationToken.None);

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Index", redirectResult.PageName);
    }

    [Fact]
    public async Task OnPostUnpaidAsync_WhenUserLoggedIn_MarksAsUnpaidAndReturnsPage()
    {
        // Arrange
        _mockDoctorService.Setup(s => s.MarkAsUnpaidAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockDoctorService.Setup(s => s.GetBillableAppointmentsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AppointmentDto>());

        var model = new BillModel(_mockDoctorService.Object);
        var httpContext = CreateHttpContext(userId: 1);
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnPostUnpaidAsync(1, CancellationToken.None);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Equal("Appointment marked as unpaid.", model.Message);
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
