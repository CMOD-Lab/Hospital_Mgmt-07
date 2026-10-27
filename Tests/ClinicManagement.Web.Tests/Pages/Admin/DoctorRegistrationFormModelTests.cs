using System;
using System.Collections.Generic;
using System.Linq;
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

public class DoctorRegistrationFormModelTests
{
    private readonly Mock<AdminService> _mockAdminService;
    private readonly Mock<ILogger<DoctorRegistrationFormModel>> _mockLogger;

    public DoctorRegistrationFormModelTests()
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
        _mockLogger = new Mock<ILogger<DoctorRegistrationFormModel>>();
    }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        // Arrange & Act
        var model = new DoctorRegistrationFormModel(_mockAdminService.Object, _mockLogger.Object);

        // Assert
        Assert.NotNull(model);
    }

    [Fact]
    public void Departments_DefaultValue_IsEmpty()
    {
        // Arrange
        var model = new DoctorRegistrationFormModel(_mockAdminService.Object, _mockLogger.Object);

        // Assert
        Assert.Empty(model.Departments);
    }

    [Fact]
    public void Message_DefaultValue_IsNull()
    {
        // Arrange
        var model = new DoctorRegistrationFormModel(_mockAdminService.Object, _mockLogger.Object);

        // Assert
        Assert.Null(model.Message);
    }

    [Fact]
    public void IsSuccess_DefaultValue_IsFalse()
    {
        // Arrange
        var model = new DoctorRegistrationFormModel(_mockAdminService.Object, _mockLogger.Object);

        // Assert
        Assert.False(model.IsSuccess);
    }

    [Fact]
    public async Task OnGetAsync_WhenUserNotLoggedIn_RedirectsToIndex()
    {
        // Arrange
        var model = new DoctorRegistrationFormModel(_mockAdminService.Object, _mockLogger.Object);
        var httpContext = CreateHttpContext();
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Index", redirectResult.PageName);
    }

    [Fact]
    public async Task OnGetAsync_WhenUserLoggedIn_ReturnsPageWithDepartments()
    {
        // Arrange
        var departments = new List<DepartmentDto>
        {
            new DepartmentDto { DeptNo = 1, DeptName = "Cardiology" },
            new DepartmentDto { DeptNo = 2, DeptName = "Neurology" }
        };
        _mockAdminService.Setup(s => s.GetDepartmentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(departments);

        var model = new DoctorRegistrationFormModel(_mockAdminService.Object, _mockLogger.Object);
        var httpContext = CreateHttpContext(userId: 1);
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Equal(2, model.Departments.Count());
    }

    [Fact]
    public async Task OnPostAsync_WhenAddDoctorSucceeds_ReturnsPageWithSuccessMessage()
    {
        // Arrange
        _mockAdminService.Setup(s => s.GetDepartmentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DepartmentDto>());
        _mockAdminService.Setup(s => s.AddDoctorAsync(It.IsAny<AddDoctorDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var model = new DoctorRegistrationFormModel(_mockAdminService.Object, _mockLogger.Object);
        var httpContext = CreateHttpContext(userId: 1);
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnPostAsync(
            "Dr. Smith", "smith@clinic.com", "password", "1980-01-01",
            1, "M", "1234567890", "123 Main St",
            10, 80000, 150, "Cardiology", "MBBS",
            CancellationToken.None);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.True(model.IsSuccess);
        Assert.Equal("Doctor registered successfully!", model.Message);
    }

    [Fact]
    public async Task OnPostAsync_WhenAddDoctorFails_ReturnsPageWithErrorMessage()
    {
        // Arrange
        _mockAdminService.Setup(s => s.GetDepartmentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DepartmentDto>());
        _mockAdminService.Setup(s => s.AddDoctorAsync(It.IsAny<AddDoctorDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var model = new DoctorRegistrationFormModel(_mockAdminService.Object, _mockLogger.Object);
        var httpContext = CreateHttpContext(userId: 1);
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnPostAsync(
            "Dr. Smith", "existing@clinic.com", "password", "1980-01-01",
            1, "M", "1234567890", "123 Main St",
            10, 80000, 150, "Cardiology", "MBBS",
            CancellationToken.None);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.False(model.IsSuccess);
        Assert.Equal("Error: Email may already exist or invalid data.", model.Message);
    }

    [Fact]
    public async Task OnPostAsync_WithEmptyGender_UsesDefaultMaleGender()
    {
        // Arrange
        AddDoctorDto? capturedDto = null;
        _mockAdminService.Setup(s => s.GetDepartmentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DepartmentDto>());
        _mockAdminService.Setup(s => s.AddDoctorAsync(It.IsAny<AddDoctorDto>(), It.IsAny<CancellationToken>()))
            .Callback<AddDoctorDto, CancellationToken>((dto, _) => capturedDto = dto)
            .ReturnsAsync(true);

        var model = new DoctorRegistrationFormModel(_mockAdminService.Object, _mockLogger.Object);
        var httpContext = CreateHttpContext(userId: 1);
        model.PageContext = CreatePageContext(httpContext);

        // Act
        await model.OnPostAsync(
            "Dr. Smith", "smith@clinic.com", "password", "1980-01-01",
            1, "", "1234567890", "123 Main St",
            10, 80000, 150, "Cardiology", "MBBS",
            CancellationToken.None);

        // Assert
        Assert.NotNull(capturedDto);
        Assert.Equal('M', capturedDto!.Gender);
    }

    [Fact]
    public async Task OnPostAsync_LoadsDepartmentsBeforeProcessing()
    {
        // Arrange
        var departments = new List<DepartmentDto>
        {
            new DepartmentDto { DeptNo = 1, DeptName = "Cardiology" }
        };
        _mockAdminService.Setup(s => s.GetDepartmentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(departments);
        _mockAdminService.Setup(s => s.AddDoctorAsync(It.IsAny<AddDoctorDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var model = new DoctorRegistrationFormModel(_mockAdminService.Object, _mockLogger.Object);
        var httpContext = CreateHttpContext(userId: 1);
        model.PageContext = CreatePageContext(httpContext);

        // Act
        await model.OnPostAsync(
            "Dr. Smith", "smith@clinic.com", "password", "1980-01-01",
            1, "M", "1234567890", "123 Main St",
            10, 80000, 150, "Cardiology", "MBBS",
            CancellationToken.None);

        // Assert
        Assert.Single(model.Departments);
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
