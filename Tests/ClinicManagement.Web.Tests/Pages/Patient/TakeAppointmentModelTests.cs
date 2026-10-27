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

public class TakeAppointmentModelTests
{
    private readonly Mock<PatientService> _mockPatientService;

    public TakeAppointmentModelTests()
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
        var model = new TakeAppointmentModel(_mockPatientService.Object);

        // Assert
        Assert.NotNull(model);
    }

    [Fact]
    public void Departments_DefaultValue_IsEmpty()
    {
        // Arrange
        var model = new TakeAppointmentModel(_mockPatientService.Object);

        // Assert
        Assert.Empty(model.Departments);
    }

    [Fact]
    public void Doctors_DefaultValue_IsEmpty()
    {
        // Arrange
        var model = new TakeAppointmentModel(_mockPatientService.Object);

        // Assert
        Assert.Empty(model.Doctors);
    }

    [Fact]
    public void SelectedDept_DefaultValue_IsNull()
    {
        // Arrange
        var model = new TakeAppointmentModel(_mockPatientService.Object);

        // Assert
        Assert.Null(model.SelectedDept);
    }

    [Fact]
    public async Task OnGetAsync_WhenUserNotLoggedIn_RedirectsToIndex()
    {
        // Arrange
        var model = new TakeAppointmentModel(_mockPatientService.Object);
        var httpContext = CreateHttpContext();
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnGetAsync(null, CancellationToken.None);

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Index", redirectResult.PageName);
    }

    [Fact]
    public async Task OnGetAsync_WhenUserLoggedInWithNoDept_ReturnsPageWithDepartments()
    {
        // Arrange
        var departments = new List<DepartmentDto>
        {
            new DepartmentDto { DeptNo = 1, DeptName = "Cardiology" },
            new DepartmentDto { DeptNo = 2, DeptName = "Neurology" }
        };
        _mockPatientService.Setup(s => s.GetDepartmentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(departments);

        var model = new TakeAppointmentModel(_mockPatientService.Object);
        var httpContext = CreateHttpContext(userId: 1);
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnGetAsync(null, CancellationToken.None);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Equal(2, model.Departments.Count());
        Assert.Empty(model.Doctors);
        Assert.Null(model.SelectedDept);
    }

    [Fact]
    public async Task OnGetAsync_WhenUserLoggedInWithSelectedDept_ReturnsPageWithDoctors()
    {
        // Arrange
        var departments = new List<DepartmentDto>
        {
            new DepartmentDto { DeptNo = 1, DeptName = "Cardiology" }
        };
        var doctors = new List<DoctorListDto>
        {
            new DoctorListDto { DoctorId = 1, Name = "Dr. Smith" },
            new DoctorListDto { DoctorId = 2, Name = "Dr. Jones" }
        };
        _mockPatientService.Setup(s => s.GetDepartmentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(departments);
        _mockPatientService.Setup(s => s.GetDoctorsByDepartmentAsync("Cardiology", It.IsAny<CancellationToken>()))
            .ReturnsAsync(doctors);

        var model = new TakeAppointmentModel(_mockPatientService.Object);
        var httpContext = CreateHttpContext(userId: 1);
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnGetAsync("Cardiology", CancellationToken.None);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Equal(2, model.Doctors.Count());
        Assert.Equal("Cardiology", model.SelectedDept);
    }

    [Fact]
    public async Task OnGetAsync_WhenSelectedDeptIsEmpty_DoesNotFetchDoctors()
    {
        // Arrange
        _mockPatientService.Setup(s => s.GetDepartmentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DepartmentDto>());

        var model = new TakeAppointmentModel(_mockPatientService.Object);
        var httpContext = CreateHttpContext(userId: 1);
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnGetAsync(string.Empty, CancellationToken.None);

        // Assert
        Assert.IsType<PageResult>(result);
        _mockPatientService.Verify(s => s.GetDoctorsByDepartmentAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
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
