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

public class ManageClinicModelTests
{
    private readonly Mock<AdminService> _mockAdminService;
    private readonly Mock<ILogger<ManageClinicModel>> _mockLogger;

    public ManageClinicModelTests()
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
        _mockLogger = new Mock<ILogger<ManageClinicModel>>();
    }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        // Arrange & Act
        var model = new ManageClinicModel(_mockAdminService.Object, _mockLogger.Object);

        // Assert
        Assert.NotNull(model);
    }

    [Fact]
    public void Doctors_DefaultValue_IsEmpty()
    {
        // Arrange
        var model = new ManageClinicModel(_mockAdminService.Object, _mockLogger.Object);

        // Assert
        Assert.Empty(model.Doctors);
    }

    [Fact]
    public void Patients_DefaultValue_IsEmpty()
    {
        // Arrange
        var model = new ManageClinicModel(_mockAdminService.Object, _mockLogger.Object);

        // Assert
        Assert.Empty(model.Patients);
    }

    [Fact]
    public void Staff_DefaultValue_IsEmpty()
    {
        // Arrange
        var model = new ManageClinicModel(_mockAdminService.Object, _mockLogger.Object);

        // Assert
        Assert.Empty(model.Staff);
    }

    [Fact]
    public async Task OnGetAsync_WhenUserNotLoggedIn_RedirectsToIndex()
    {
        // Arrange
        var model = new ManageClinicModel(_mockAdminService.Object, _mockLogger.Object);
        var httpContext = CreateHttpContext();
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnGetAsync(null, CancellationToken.None);

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Index", redirectResult.PageName);
    }

    [Fact]
    public async Task OnGetAsync_WhenUserLoggedIn_ReturnsPageWithData()
    {
        // Arrange
        var doctors = new List<DoctorListDto> { new DoctorListDto { DoctorId = 1, Name = "Dr. Smith" } };
        var patients = new List<PatientListDto> { new PatientListDto { PatientId = 1, Name = "John Doe" } };
        var staff = new List<StaffListDto> { new StaffListDto { StaffId = 1, Name = "Jane Nurse" } };

        _mockAdminService.Setup(s => s.GetDoctorsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(doctors);
        _mockAdminService.Setup(s => s.GetPatientsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(patients);
        _mockAdminService.Setup(s => s.GetStaffAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(staff);

        var model = new ManageClinicModel(_mockAdminService.Object, _mockLogger.Object);
        var httpContext = CreateHttpContext(userId: 1);
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnGetAsync(null, CancellationToken.None);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Single(model.Doctors);
        Assert.Single(model.Patients);
        Assert.Single(model.Staff);
    }

    [Fact]
    public async Task OnGetAsync_WithDoctorSearch_PassesSearchToService()
    {
        // Arrange
        _mockAdminService.Setup(s => s.GetDoctorsAsync("Smith", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DoctorListDto>());
        _mockAdminService.Setup(s => s.GetPatientsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PatientListDto>());
        _mockAdminService.Setup(s => s.GetStaffAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<StaffListDto>());

        var model = new ManageClinicModel(_mockAdminService.Object, _mockLogger.Object);
        var httpContext = CreateHttpContext(userId: 1);
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnGetAsync("Smith", CancellationToken.None);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Equal("Smith", model.DoctorSearch);
        _mockAdminService.Verify(s => s.GetDoctorsAsync("Smith", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task OnPostDeleteDoctorAsync_WhenDeleteSucceeds_ReturnsPageWithSuccessMessage()
    {
        // Arrange
        _mockAdminService.Setup(s => s.DeleteDoctorAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockAdminService.Setup(s => s.GetDoctorsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DoctorListDto>());
        _mockAdminService.Setup(s => s.GetPatientsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PatientListDto>());
        _mockAdminService.Setup(s => s.GetStaffAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<StaffListDto>());

        var model = new ManageClinicModel(_mockAdminService.Object, _mockLogger.Object);
        var httpContext = CreateHttpContext(userId: 1);
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnPostDeleteDoctorAsync(1, CancellationToken.None);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Equal("Doctor removed successfully.", model.Message);
    }

    [Fact]
    public async Task OnPostDeleteDoctorAsync_WhenDeleteFails_ReturnsPageWithErrorMessage()
    {
        // Arrange
        _mockAdminService.Setup(s => s.DeleteDoctorAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockAdminService.Setup(s => s.GetDoctorsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DoctorListDto>());
        _mockAdminService.Setup(s => s.GetPatientsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PatientListDto>());
        _mockAdminService.Setup(s => s.GetStaffAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<StaffListDto>());

        var model = new ManageClinicModel(_mockAdminService.Object, _mockLogger.Object);
        var httpContext = CreateHttpContext(userId: 1);
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnPostDeleteDoctorAsync(1, CancellationToken.None);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Equal("Error removing doctor.", model.Message);
    }

    [Fact]
    public async Task OnPostDeleteStaffAsync_WhenDeleteSucceeds_ReturnsPageWithSuccessMessage()
    {
        // Arrange
        _mockAdminService.Setup(s => s.DeleteStaffAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockAdminService.Setup(s => s.GetDoctorsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DoctorListDto>());
        _mockAdminService.Setup(s => s.GetPatientsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PatientListDto>());
        _mockAdminService.Setup(s => s.GetStaffAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<StaffListDto>());

        var model = new ManageClinicModel(_mockAdminService.Object, _mockLogger.Object);
        var httpContext = CreateHttpContext(userId: 1);
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnPostDeleteStaffAsync(1, CancellationToken.None);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Equal("Staff member deleted successfully.", model.Message);
    }

    [Fact]
    public async Task OnPostDeleteStaffAsync_WhenDeleteFails_ReturnsPageWithErrorMessage()
    {
        // Arrange
        _mockAdminService.Setup(s => s.DeleteStaffAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockAdminService.Setup(s => s.GetDoctorsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DoctorListDto>());
        _mockAdminService.Setup(s => s.GetPatientsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PatientListDto>());
        _mockAdminService.Setup(s => s.GetStaffAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<StaffListDto>());

        var model = new ManageClinicModel(_mockAdminService.Object, _mockLogger.Object);
        var httpContext = CreateHttpContext(userId: 1);
        model.PageContext = CreatePageContext(httpContext);

        // Act
        var result = await model.OnPostDeleteStaffAsync(1, CancellationToken.None);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Equal("Error deleting staff.", model.Message);
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
