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

public class TreatmentHistoryModelTests
{
    private readonly Mock<PatientService> _mockPatientService;

    public TreatmentHistoryModelTests()
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
        var model = new TreatmentHistoryModel(_mockPatientService.Object);

        // Assert
        Assert.NotNull(model);
    }

    [Fact]
    public void History_DefaultValue_IsEmpty()
    {
        // Arrange
        var model = new TreatmentHistoryModel(_mockPatientService.Object);

        // Assert
        Assert.Empty(model.History);
    }

    [Fact]
    public async Task OnGetAsync_WhenUserNotLoggedIn_RedirectsToIndex()
    {
        // Arrange
        var model = new TreatmentHistoryModel(_mockPatientService.Object);
        var httpContext = CreateHttpContext();
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Index", redirectResult.PageName);
    }

    [Fact]
    public async Task OnGetAsync_WhenUserLoggedIn_ReturnsPageWithHistory()
    {
        // Arrange
        var history = new List<AppointmentDto>
        {
            new AppointmentDto { AppointId = 1, DoctorName = "Dr. Smith" },
            new AppointmentDto { AppointId = 2, DoctorName = "Dr. Jones" }
        };
        _mockPatientService.Setup(s => s.GetTreatmentHistoryAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(history);

        var model = new TreatmentHistoryModel(_mockPatientService.Object);
        var httpContext = CreateHttpContext(userId: 1);
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Equal(2, model.History.Count());
    }

    [Fact]
    public async Task OnGetAsync_WhenNoHistory_ReturnsPageWithEmptyHistory()
    {
        // Arrange
        _mockPatientService.Setup(s => s.GetTreatmentHistoryAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AppointmentDto>());

        var model = new TreatmentHistoryModel(_mockPatientService.Object);
        var httpContext = CreateHttpContext(userId: 1);
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Empty(model.History);
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
