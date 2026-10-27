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

public class AddStaffModelTests
{
    private readonly Mock<AdminService> _mockAdminService;
    private readonly Mock<ILogger<AddStaffModel>> _mockLogger;

    public AddStaffModelTests()
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
        _mockLogger = new Mock<ILogger<AddStaffModel>>();
    }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        // Arrange & Act
        var model = new AddStaffModel(_mockAdminService.Object, _mockLogger.Object);

        // Assert
        Assert.NotNull(model);
    }

    [Fact]
    public void Message_DefaultValue_IsNull()
    {
        // Arrange
        var model = new AddStaffModel(_mockAdminService.Object, _mockLogger.Object);

        // Assert
        Assert.Null(model.Message);
    }

    [Fact]
    public void IsSuccess_DefaultValue_IsFalse()
    {
        // Arrange
        var model = new AddStaffModel(_mockAdminService.Object, _mockLogger.Object);

        // Assert
        Assert.False(model.IsSuccess);
    }

    [Fact]
    public void OnGet_WhenUserNotLoggedIn_RedirectsToIndex()
    {
        // Arrange
        var model = new AddStaffModel(_mockAdminService.Object, _mockLogger.Object);
        var httpContext = CreateHttpContext();
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = model.OnGet();

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Index", redirectResult.PageName);
    }

    [Fact]
    public void OnGet_WhenUserLoggedIn_ReturnsPage()
    {
        // Arrange
        var model = new AddStaffModel(_mockAdminService.Object, _mockLogger.Object);
        var httpContext = CreateHttpContext(userId: 1);
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = model.OnGet();

        // Assert
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_WhenAddStaffSucceeds_ReturnsPageWithSuccessMessage()
    {
        // Arrange
        _mockAdminService.Setup(s => s.AddStaffAsync(It.IsAny<AddStaffDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var model = new AddStaffModel(_mockAdminService.Object, _mockLogger.Object);
        var httpContext = CreateHttpContext(userId: 1);
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnPostAsync(
            "John Doe", "1990-01-01", "1234567890", "M",
            "123 Main St", 50000, "BSc Nursing", "Nurse",
            CancellationToken.None);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.True(model.IsSuccess);
        Assert.Equal("Staff member added successfully!", model.Message);
    }

    [Fact]
    public async Task OnPostAsync_WhenAddStaffFails_ReturnsPageWithErrorMessage()
    {
        // Arrange
        _mockAdminService.Setup(s => s.AddStaffAsync(It.IsAny<AddStaffDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var model = new AddStaffModel(_mockAdminService.Object, _mockLogger.Object);
        var httpContext = CreateHttpContext(userId: 1);
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnPostAsync(
            "John Doe", "1990-01-01", "1234567890", "M",
            "123 Main St", 50000, "BSc Nursing", "Nurse",
            CancellationToken.None);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.False(model.IsSuccess);
        Assert.Equal("Error adding staff member.", model.Message);
    }

    [Fact]
    public async Task OnPostAsync_WithEmptyGender_UsesDefaultMaleGender()
    {
        // Arrange
        AddStaffDto? capturedDto = null;
        _mockAdminService.Setup(s => s.AddStaffAsync(It.IsAny<AddStaffDto>(), It.IsAny<CancellationToken>()))
            .Callback<AddStaffDto, CancellationToken>((dto, _) => capturedDto = dto)
            .ReturnsAsync(true);

        var model = new AddStaffModel(_mockAdminService.Object, _mockLogger.Object);
        var httpContext = CreateHttpContext(userId: 1);
        model.PageContext = CreatePageContext(httpContext);

        // Act
        await model.OnPostAsync(
            "John Doe", "1990-01-01", "1234567890", "",
            "123 Main St", 50000, "BSc Nursing", "Nurse",
            CancellationToken.None);

        // Assert
        Assert.NotNull(capturedDto);
        Assert.Equal('M', capturedDto!.Gender);
    }

    [Fact]
    public async Task OnPostAsync_WithFemaleGender_UsesFemaleGender()
    {
        // Arrange
        AddStaffDto? capturedDto = null;
        _mockAdminService.Setup(s => s.AddStaffAsync(It.IsAny<AddStaffDto>(), It.IsAny<CancellationToken>()))
            .Callback<AddStaffDto, CancellationToken>((dto, _) => capturedDto = dto)
            .ReturnsAsync(true);

        var model = new AddStaffModel(_mockAdminService.Object, _mockLogger.Object);
        var httpContext = CreateHttpContext(userId: 1);
        model.PageContext = CreatePageContext(httpContext);

        // Act
        await model.OnPostAsync(
            "Jane Doe", "1990-01-01", "1234567890", "F",
            "123 Main St", 50000, "BSc Nursing", "Nurse",
            CancellationToken.None);

        // Assert
        Assert.NotNull(capturedDto);
        Assert.Equal('F', capturedDto!.Gender);
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
