using System;
using System.Threading;
using System.Threading.Tasks;
using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Web.Pages.Admin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClinicManagement.Web.Tests.Pages.Admin;

public class AdminHomeModelTests
{
    private readonly Mock<AdminService> _mockAdminService;
    private readonly Mock<ILogger<AdminHomeModel>> _mockLogger;

    public AdminHomeModelTests()
    {
        var doctorRepoMock = new Mock<IDoctorRepository>();
        var patientRepoMock = new Mock<IPatientRepository>();
        var staffRepoMock = new Mock<IStaffRepository>();
        var appointmentRepoMock = new Mock<IAppointmentRepository>();
        var deptRepoMock = new Mock<IDepartmentRepository>();
        var loginRepoMock = new Mock<ILoginRepository>();
        var loggerMock = new Mock<ILogger<AdminService>>();
        _mockAdminService = new Mock<AdminService>(
            doctorRepoMock.Object, staffRepoMock.Object, patientRepoMock.Object,
            appointmentRepoMock.Object, deptRepoMock.Object, loginRepoMock.Object, loggerMock.Object);
        _mockLogger = new Mock<ILogger<AdminHomeModel>>();
    }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        // Arrange & Act
        var model = new AdminHomeModel(_mockAdminService.Object, _mockLogger.Object);

        // Assert
        Assert.NotNull(model);
    }

    [Fact]
    public void Stats_DefaultValue_IsNull()
    {
        // Arrange
        var model = new AdminHomeModel(_mockAdminService.Object, _mockLogger.Object);

        // Assert
        Assert.Null(model.Stats);
    }

    [Fact]
    public async Task OnGetAsync_WhenUserNotLoggedIn_RedirectsToIndex()
    {
        // Arrange
        var model = new AdminHomeModel(_mockAdminService.Object, _mockLogger.Object);
        var httpContext = CreateHttpContext();
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Index", redirectResult.PageName);
    }

    [Fact]
    public async Task OnGetAsync_WhenUserLoggedIn_ReturnsPageWithStats()
    {
        // Arrange
        var statsDto = new AdminHomeDto
        {
            TotalDoctors = 10,
            TotalPatients = 50,
            TotalIncome = 5000.0
        };
        _mockAdminService.Setup(s => s.GetAdminHomeInfoAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(statsDto);

        var model = new AdminHomeModel(_mockAdminService.Object, _mockLogger.Object);
        var httpContext = CreateHttpContext(userId: 1);
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.NotNull(model.Stats);
        Assert.Equal(10, model.Stats!.TotalDoctors);
        Assert.Equal(50, model.Stats.TotalPatients);
    }

    [Fact]
    public async Task OnGetAsync_WhenStatsIsNull_ReturnsPage()
    {
        // Arrange
        _mockAdminService.Setup(s => s.GetAdminHomeInfoAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((AdminHomeDto?)null);

        var model = new AdminHomeModel(_mockAdminService.Object, _mockLogger.Object);
        var httpContext = CreateHttpContext(userId: 1);
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Null(model.Stats);
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
