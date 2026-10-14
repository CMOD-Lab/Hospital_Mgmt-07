using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClinicManagement.UnitTests.Services;

/// <summary>
/// Unit tests for AdminService.
/// </summary>
public class AdminServiceTests
{
    private readonly Mock<IDoctorRepository> _doctorRepositoryMock;
    private readonly Mock<IPatientRepository> _patientRepositoryMock;
    private readonly Mock<IAppointmentRepository> _appointmentRepositoryMock;
    private readonly Mock<IDepartmentRepository> _departmentRepositoryMock;
    private readonly Mock<ILogger<AdminService>> _loggerMock;
    private readonly AdminService _adminService;

    public AdminServiceTests()
    {
        _doctorRepositoryMock = new Mock<IDoctorRepository>();
        _patientRepositoryMock = new Mock<IPatientRepository>();
        _appointmentRepositoryMock = new Mock<IAppointmentRepository>();
        _departmentRepositoryMock = new Mock<IDepartmentRepository>();
        _loggerMock = new Mock<ILogger<AdminService>>();
        _adminService = new AdminService(
            _doctorRepositoryMock.Object,
            _patientRepositoryMock.Object,
            _appointmentRepositoryMock.Object,
            _departmentRepositoryMock.Object,
            _loggerMock.Object);
    }

    // ─── GetAdminHomeInfoAsync ────────────────────────────────────────────────

    [Fact]
    public async Task GetAdminHomeInfoAsync_WithData_ReturnsCorrectCounts()
    {
        // Arrange
        var doctors = new List<Doctor>
        {
            new Doctor { DoctorId = 1, Name = "Dr. A", Status = 1 },
            new Doctor { DoctorId = 2, Name = "Dr. B", Status = 1 }
        };

        var patients = new List<Patient>
        {
            new Patient { PatientId = 1, Name = "Patient A" },
            new Patient { PatientId = 2, Name = "Patient B" },
            new Patient { PatientId = 3, Name = "Patient C" }
        };

        var appointments = new List<Appointment>
        {
            new Appointment { AppointId = 1, BillStatus = "Paid", BillAmount = 500.0, Date = DateTime.Now, AppointmentStatus = 3 },
            new Appointment { AppointId = 2, BillStatus = "Paid", BillAmount = 300.0, Date = DateTime.Now.AddDays(-1), AppointmentStatus = 3 },
            new Appointment { AppointId = 3, BillStatus = "Unpaid", BillAmount = 200.0, Date = DateTime.Now.AddDays(-2), AppointmentStatus = 2 }
        };

        var departments = new List<Department>
        {
            new Department
            {
                DeptNo = 1,
                DeptName = "Cardiology",
                Description = "Heart care",
                Doctors = new List<Doctor>
                {
                    new Doctor { DoctorId = 1, Status = 1 },
                    new Doctor { DoctorId = 2, Status = 0 }
                }
            }
        };

        _doctorRepositoryMock.Setup(r => r.GetActiveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(doctors);
        _patientRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(patients);
        _appointmentRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(appointments);
        _departmentRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(departments);

        // Act
        var result = await _adminService.GetAdminHomeInfoAsync();

        // Assert
        result.Should().NotBeNull();
        result.TotalDoctors.Should().Be(2);
        result.TotalPatients.Should().Be(3);
        result.TotalIncome.Should().Be(800.0); // Only paid appointments
    }

    [Fact]
    public async Task GetAdminHomeInfoAsync_WithNoPaidAppointments_TotalIncomeIsZero()
    {
        // Arrange
        var appointments = new List<Appointment>
        {
            new Appointment { AppointId = 1, BillStatus = "Unpaid", BillAmount = 500.0, Date = DateTime.Now, AppointmentStatus = 2 }
        };

        _doctorRepositoryMock.Setup(r => r.GetActiveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Doctor>());
        _patientRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Patient>());
        _appointmentRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(appointments);
        _departmentRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Department>());

        // Act
        var result = await _adminService.GetAdminHomeInfoAsync();

        // Assert
        result.TotalIncome.Should().Be(0.0);
    }

    [Fact]
    public async Task GetAdminHomeInfoAsync_DepartmentDoctorCount_OnlyCountsActiveDoctors()
    {
        // Arrange
        var departments = new List<Department>
        {
            new Department
            {
                DeptNo = 1,
                DeptName = "Neurology",
                Doctors = new List<Doctor>
                {
                    new Doctor { DoctorId = 1, Status = 1 }, // Active
                    new Doctor { DoctorId = 2, Status = 1 }, // Active
                    new Doctor { DoctorId = 3, Status = 0 }  // Left
                }
            }
        };

        _doctorRepositoryMock.Setup(r => r.GetActiveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Doctor>());
        _patientRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Patient>());
        _appointmentRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Appointment>());
        _departmentRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(departments);

        // Act
        var result = await _adminService.GetAdminHomeInfoAsync();

        // Assert
        result.Departments.Should().HaveCount(1);
        result.Departments.First().DoctorCount.Should().Be(2);
    }

    [Fact]
    public async Task GetAdminHomeInfoAsync_RecentAppointments_LimitedToTen()
    {
        // Arrange
        var appointments = Enumerable.Range(1, 15).Select(i => new Appointment
        {
            AppointId = i,
            Date = DateTime.Now.AddDays(-i),
            AppointmentStatus = 1,
            BillStatus = "Paid",
            BillAmount = 100.0
        }).ToList();

        _doctorRepositoryMock.Setup(r => r.GetActiveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Doctor>());
        _patientRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Patient>());
        _appointmentRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(appointments);
        _departmentRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Department>());

        // Act
        var result = await _adminService.GetAdminHomeInfoAsync();

        // Assert
        result.RecentAppointments.Should().HaveCount(10);
    }

    [Fact]
    public async Task GetAdminHomeInfoAsync_WhenRepositoryThrows_ReturnsEmptyDto()
    {
        // Arrange
        _doctorRepositoryMock
            .Setup(r => r.GetActiveAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _adminService.GetAdminHomeInfoAsync();

        // Assert
        result.Should().NotBeNull();
        result.TotalDoctors.Should().Be(0);
        result.TotalPatients.Should().Be(0);
        result.TotalIncome.Should().Be(0.0);
    }

    [Fact]
    public async Task GetAdminHomeInfoAsync_WithNullBillAmount_ExcludesFromIncome()
    {
        // Arrange
        var appointments = new List<Appointment>
        {
            new Appointment { AppointId = 1, BillStatus = "Paid", BillAmount = null, Date = DateTime.Now, AppointmentStatus = 3 },
            new Appointment { AppointId = 2, BillStatus = "Paid", BillAmount = 400.0, Date = DateTime.Now, AppointmentStatus = 3 }
        };

        _doctorRepositoryMock.Setup(r => r.GetActiveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Doctor>());
        _patientRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Patient>());
        _appointmentRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(appointments);
        _departmentRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Department>());

        // Act
        var result = await _adminService.GetAdminHomeInfoAsync();

        // Assert
        result.TotalIncome.Should().Be(400.0);
    }

    [Fact]
    public async Task GetAdminHomeInfoAsync_DepartmentWithNullDoctors_DoctorCountIsZero()
    {
        // Arrange
        var departments = new List<Department>
        {
            new Department { DeptNo = 1, DeptName = "Radiology", Doctors = null! }
        };

        _doctorRepositoryMock.Setup(r => r.GetActiveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Doctor>());
        _patientRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Patient>());
        _appointmentRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Appointment>());
        _departmentRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(departments);

        // Act
        var result = await _adminService.GetAdminHomeInfoAsync();

        // Assert
        result.Departments.First().DoctorCount.Should().Be(0);
    }
}
