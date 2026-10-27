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

public class DoctorHomeModelTests
{
    private readonly Mock<DoctorService> _mockDoctorService;

    public DoctorHomeModelTests()
    {
        var appointmentRepoMock = new Mock<IAppointmentRepository>();
        var doctorRepoMock = new Mock<IDoctorRepository>();
        var loggerMock = new Mock<ILogger<DoctorService>>();
        _mockDoctorService = new Mock<DoctorService>(
            doctorRepoMock.Object, appointmentRepoMock.Object, loggerMock.Object);
    }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        // Arrange & Act
        var model = new DoctorHomeModel(_mockDoctorService.Object);

        // Assert
        Assert.NotNull(model);
    }

    [Fact]
    public void DoctorInfo_DefaultValue_IsNull()
    {
        // Arrange
        var model = new DoctorHomeModel(_mockDoctorService.Object);

        // Assert
        Assert.Null(model.DoctorInfo);
    }

    [Fact]
    public async Task OnGetAsync_WhenUserNotLoggedIn_RedirectsToIndex()
    {
        // Arrange
        var model = new DoctorHomeModel(_mockDoctorService.Object);
        var httpContext = CreateHttpContext();
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Index", redirectResult.PageName);
    }

    [Fact]
    public async Task OnGetAsync_WhenUserLoggedIn_ReturnsPageWithDoctorInfo()
    {
        // Arrange
        var doctorDto = new DoctorProfileDto { DoctorId = 1, Name = "Dr. Smith", Specialization = "Cardiology" };
        _mockDoctorService.Setup(s => s.GetDoctorInfoAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(doctorDto);

        var model = new DoctorHomeModel(_mockDoctorService.Object);
        var httpContext = CreateHttpContext(userId: 1);
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.NotNull(model.DoctorInfo);
        Assert.Equal("Dr. Smith", model.DoctorInfo!.Name);
    }

    [Fact]
    public async Task OnGetAsync_WhenDoctorInfoIsNull_ReturnsPage()
    {
        // Arrange
        _mockDoctorService.Setup(s => s.GetDoctorInfoAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DoctorProfileDto?)null);

        var model = new DoctorHomeModel(_mockDoctorService.Object);
        var httpContext = CreateHttpContext(userId: 1);
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Null(model.DoctorInfo);
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
