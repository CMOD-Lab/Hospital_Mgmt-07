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
/// Unit tests for AppointmentService.
/// </summary>
public class AppointmentServiceTests
{
    private readonly Mock<IAppointmentRepository> _appointmentRepositoryMock;
    private readonly Mock<IDoctorRepository> _doctorRepositoryMock;
    private readonly Mock<ILogger<AppointmentService>> _loggerMock;
    private readonly AppointmentService _appointmentService;

    public AppointmentServiceTests()
    {
        _appointmentRepositoryMock = new Mock<IAppointmentRepository>();
        _doctorRepositoryMock = new Mock<IDoctorRepository>();
        _loggerMock = new Mock<ILogger<AppointmentService>>();
        _appointmentService = new AppointmentService(
            _appointmentRepositoryMock.Object,
            _doctorRepositoryMock.Object,
            _loggerMock.Object);
    }

    private static Appointment CreateAppointment(int id = 1, int status = 2) => new Appointment
    {
        AppointId = id,
        DoctorId = 10,
        PatientId = 20,
        Date = DateTime.Now,
        AppointmentStatus = status,
        BillAmount = 500.0,
        BillStatus = "Unpaid",
        DoctorNotification = 2,
        PatientNotification = 2,
        FeedbackStatus = 2,
        Doctor = new Doctor { DoctorId = 10, Name = "Dr. Smith" },
        Patient = new Patient { PatientId = 20, Name = "John Doe" }
    };

    // ─── GetCurrentAppointmentAsync ───────────────────────────────────────────

    [Fact]
    public async Task GetCurrentAppointmentAsync_WhenExists_ReturnsDto()
    {
        // Arrange
        var appointment = CreateAppointment();
        _appointmentRepositoryMock
            .Setup(r => r.GetCurrentByPatientIdAsync(20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointment);

        // Act
        var result = await _appointmentService.GetCurrentAppointmentAsync(20);

        // Assert
        result.Should().NotBeNull();
        result!.AppointId.Should().Be(1);
        result.DoctorName.Should().Be("Dr. Smith");
        result.PatientName.Should().Be("John Doe");
    }

    [Fact]
    public async Task GetCurrentAppointmentAsync_WhenNotFound_ReturnsNull()
    {
        // Arrange
        _appointmentRepositoryMock
            .Setup(r => r.GetCurrentByPatientIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Appointment?)null);

        // Act
        var result = await _appointmentService.GetCurrentAppointmentAsync(99);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetCurrentAppointmentAsync_WhenRepositoryThrows_ReturnsNull()
    {
        // Arrange
        _appointmentRepositoryMock
            .Setup(r => r.GetCurrentByPatientIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _appointmentService.GetCurrentAppointmentAsync(1);

        // Assert
        result.Should().BeNull();
    }

    // ─── GetTreatmentHistoryAsync ─────────────────────────────────────────────

    [Fact]
    public async Task GetTreatmentHistoryAsync_WithHistory_ReturnsDtos()
    {
        // Arrange
        var appointments = new List<Appointment>
        {
            CreateAppointment(1, 3),
            CreateAppointment(2, 3)
        };

        _appointmentRepositoryMock
            .Setup(r => r.GetTreatmentHistoryAsync(20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointments);

        // Act
        var result = await _appointmentService.GetTreatmentHistoryAsync(20);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetTreatmentHistoryAsync_WhenRepositoryThrows_ReturnsEmpty()
    {
        // Arrange
        _appointmentRepositoryMock
            .Setup(r => r.GetTreatmentHistoryAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _appointmentService.GetTreatmentHistoryAsync(1);

        // Assert
        result.Should().BeEmpty();
    }

    // ─── GetBillHistoryAsync ──────────────────────────────────────────────────

    [Fact]
    public async Task GetBillHistoryAsync_WithBills_ReturnsDtos()
    {
        // Arrange
        var appointments = new List<Appointment> { CreateAppointment(1, 3) };
        _appointmentRepositoryMock
            .Setup(r => r.GetBillHistoryAsync(20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointments);

        // Act
        var result = await _appointmentService.GetBillHistoryAsync(20);

        // Assert
        result.Should().HaveCount(1);
        result.First().BillAmount.Should().Be(500.0);
    }

    [Fact]
    public async Task GetBillHistoryAsync_WhenRepositoryThrows_ReturnsEmpty()
    {
        // Arrange
        _appointmentRepositoryMock
            .Setup(r => r.GetBillHistoryAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _appointmentService.GetBillHistoryAsync(1);

        // Assert
        result.Should().BeEmpty();
    }

    // ─── GetFreeSlotsAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task GetFreeSlotsAsync_WithSlots_ReturnsDtos()
    {
        // Arrange
        var slots = new List<Appointment> { CreateAppointment(1, 2) };
        _appointmentRepositoryMock
            .Setup(r => r.GetFreeSlotsByDoctorAsync(10, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(slots);

        // Act
        var result = await _appointmentService.GetFreeSlotsAsync(10, 20);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetFreeSlotsAsync_WhenRepositoryThrows_ReturnsEmpty()
    {
        // Arrange
        _appointmentRepositoryMock
            .Setup(r => r.GetFreeSlotsByDoctorAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _appointmentService.GetFreeSlotsAsync(1, 2);

        // Assert
        result.Should().BeEmpty();
    }

    // ─── CreateAppointmentAsync ───────────────────────────────────────────────

    [Fact]
    public async Task CreateAppointmentAsync_WithValidDto_ReturnsTrue()
    {
        // Arrange
        var dto = new CreateAppointmentDto { DoctorId = 10, PatientId = 20, FreeSlotId = 1 };
        _appointmentRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Appointment { AppointId = 5 });

        // Act
        var result = await _appointmentService.CreateAppointmentAsync(dto);

        // Assert
        result.Should().BeTrue();
        _appointmentRepositoryMock.Verify(r => r.AddAsync(
            It.Is<Appointment>(a => a.DoctorId == 10 && a.PatientId == 20 && a.AppointmentStatus == 2),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAppointmentAsync_WhenRepositoryThrows_ReturnsFalse()
    {
        // Arrange
        var dto = new CreateAppointmentDto { DoctorId = 10, PatientId = 20 };
        _appointmentRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _appointmentService.CreateAppointmentAsync(dto);

        // Assert
        result.Should().BeFalse();
    }

    // ─── GetPendingAppointmentsAsync ──────────────────────────────────────────

    [Fact]
    public async Task GetPendingAppointmentsAsync_WithPending_ReturnsDtos()
    {
        // Arrange
        var appointments = new List<Appointment> { CreateAppointment(1, 2) };
        _appointmentRepositoryMock
            .Setup(r => r.GetPendingByDoctorIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointments);

        // Act
        var result = await _appointmentService.GetPendingAppointmentsAsync(10);

        // Assert
        result.Should().HaveCount(1);
        result.First().AppointmentStatus.Should().Be(2);
    }

    [Fact]
    public async Task GetPendingAppointmentsAsync_WhenRepositoryThrows_ReturnsEmpty()
    {
        // Arrange
        _appointmentRepositoryMock
            .Setup(r => r.GetPendingByDoctorIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _appointmentService.GetPendingAppointmentsAsync(1);

        // Assert
        result.Should().BeEmpty();
    }

    // ─── GetTodaysAppointmentsAsync ───────────────────────────────────────────

    [Fact]
    public async Task GetTodaysAppointmentsAsync_WithAppointments_ReturnsDtos()
    {
        // Arrange
        var appointments = new List<Appointment> { CreateAppointment(1, 1) };
        _appointmentRepositoryMock
            .Setup(r => r.GetTodaysByDoctorIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointments);

        // Act
        var result = await _appointmentService.GetTodaysAppointmentsAsync(10);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetTodaysAppointmentsAsync_WhenRepositoryThrows_ReturnsEmpty()
    {
        // Arrange
        _appointmentRepositoryMock
            .Setup(r => r.GetTodaysByDoctorIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _appointmentService.GetTodaysAppointmentsAsync(1);

        // Assert
        result.Should().BeEmpty();
    }

    // ─── ApproveAppointmentAsync ──────────────────────────────────────────────

    [Fact]
    public async Task ApproveAppointmentAsync_WhenExists_UpdatesStatusAndReturnsTrue()
    {
        // Arrange
        var appointment = CreateAppointment(1, 2);
        _appointmentRepositoryMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointment);

        // Act
        var result = await _appointmentService.ApproveAppointmentAsync(1);

        // Assert
        result.Should().BeTrue();
        appointment.AppointmentStatus.Should().Be(1);
        appointment.PatientNotification.Should().Be(2);
        _appointmentRepositoryMock.Verify(r => r.UpdateAsync(appointment, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ApproveAppointmentAsync_WhenNotFound_ReturnsFalse()
    {
        // Arrange
        _appointmentRepositoryMock
            .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Appointment?)null);

        // Act
        var result = await _appointmentService.ApproveAppointmentAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ApproveAppointmentAsync_WhenRepositoryThrows_ReturnsFalse()
    {
        // Arrange
        _appointmentRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _appointmentService.ApproveAppointmentAsync(1);

        // Assert
        result.Should().BeFalse();
    }

    // ─── DeleteAppointmentAsync ───────────────────────────────────────────────

    [Fact]
    public async Task DeleteAppointmentAsync_WithValidId_ReturnsTrue()
    {
        // Arrange
        _appointmentRepositoryMock
            .Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _appointmentService.DeleteAppointmentAsync(1);

        // Assert
        result.Should().BeTrue();
        _appointmentRepositoryMock.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAppointmentAsync_WhenRepositoryThrows_ReturnsFalse()
    {
        // Arrange
        _appointmentRepositoryMock
            .Setup(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _appointmentService.DeleteAppointmentAsync(1);

        // Assert
        result.Should().BeFalse();
    }

    // ─── UpdatePrescriptionAsync ──────────────────────────────────────────────

    [Fact]
    public async Task UpdatePrescriptionAsync_WhenExists_UpdatesAndReturnsTrue()
    {
        // Arrange
        var appointment = CreateAppointment(1, 1);
        var dto = new UpdatePrescriptionDto
        {
            AppointmentId = 1,
            DoctorId = 10,
            Disease = "Flu",
            Progress = "Improving",
            Prescription = "Rest and fluids"
        };

        _appointmentRepositoryMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointment);

        // Act
        var result = await _appointmentService.UpdatePrescriptionAsync(dto);

        // Assert
        result.Should().BeTrue();
        appointment.Disease.Should().Be("Flu");
        appointment.Progress.Should().Be("Improving");
        appointment.Prescription.Should().Be("Rest and fluids");
        _appointmentRepositoryMock.Verify(r => r.UpdateAsync(appointment, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdatePrescriptionAsync_WhenNotFound_ReturnsFalse()
    {
        // Arrange
        var dto = new UpdatePrescriptionDto { AppointmentId = 999, DoctorId = 10 };
        _appointmentRepositoryMock
            .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Appointment?)null);

        // Act
        var result = await _appointmentService.UpdatePrescriptionAsync(dto);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UpdatePrescriptionAsync_WhenRepositoryThrows_ReturnsFalse()
    {
        // Arrange
        var dto = new UpdatePrescriptionDto { AppointmentId = 1, DoctorId = 10 };
        _appointmentRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _appointmentService.UpdatePrescriptionAsync(dto);

        // Assert
        result.Should().BeFalse();
    }

    // ─── MarkAsPaidAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task MarkAsPaidAsync_WhenExists_SetsPaidAndReturnsTrue()
    {
        // Arrange
        var appointment = CreateAppointment(1, 1);
        _appointmentRepositoryMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointment);

        // Act
        var result = await _appointmentService.MarkAsPaidAsync(10, 1);

        // Assert
        result.Should().BeTrue();
        appointment.BillStatus.Should().Be("Paid");
        appointment.AppointmentStatus.Should().Be(3);
        appointment.FeedbackStatus.Should().Be(2);
    }

    [Fact]
    public async Task MarkAsPaidAsync_WhenNotFound_ReturnsFalse()
    {
        // Arrange
        _appointmentRepositoryMock
            .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Appointment?)null);

        // Act
        var result = await _appointmentService.MarkAsPaidAsync(10, 999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task MarkAsPaidAsync_WhenRepositoryThrows_ReturnsFalse()
    {
        // Arrange
        _appointmentRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _appointmentService.MarkAsPaidAsync(10, 1);

        // Assert
        result.Should().BeFalse();
    }

    // ─── MarkAsUnpaidAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task MarkAsUnpaidAsync_WhenExists_SetsUnpaidAndReturnsTrue()
    {
        // Arrange
        var appointment = CreateAppointment(1, 1);
        _appointmentRepositoryMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointment);

        // Act
        var result = await _appointmentService.MarkAsUnpaidAsync(10, 1);

        // Assert
        result.Should().BeTrue();
        appointment.BillStatus.Should().Be("Unpaid");
        appointment.AppointmentStatus.Should().Be(3);
        appointment.FeedbackStatus.Should().Be(2);
    }

    [Fact]
    public async Task MarkAsUnpaidAsync_WhenNotFound_ReturnsFalse()
    {
        // Arrange
        _appointmentRepositoryMock
            .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Appointment?)null);

        // Act
        var result = await _appointmentService.MarkAsUnpaidAsync(10, 999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task MarkAsUnpaidAsync_WhenRepositoryThrows_ReturnsFalse()
    {
        // Arrange
        _appointmentRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _appointmentService.MarkAsUnpaidAsync(10, 1);

        // Assert
        result.Should().BeFalse();
    }

    // ─── GetPendingFeedbackAsync ──────────────────────────────────────────────

    [Fact]
    public async Task GetPendingFeedbackAsync_WhenExists_ReturnsDto()
    {
        // Arrange
        var appointment = CreateAppointment(1, 3);
        appointment.FeedbackStatus = 2;
        _appointmentRepositoryMock
            .Setup(r => r.GetPendingFeedbackAsync(20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointment);

        // Act
        var result = await _appointmentService.GetPendingFeedbackAsync(20);

        // Assert
        result.Should().NotBeNull();
        result!.FeedbackStatus.Should().Be(2);
    }

    [Fact]
    public async Task GetPendingFeedbackAsync_WhenNotFound_ReturnsNull()
    {
        // Arrange
        _appointmentRepositoryMock
            .Setup(r => r.GetPendingFeedbackAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Appointment?)null);

        // Act
        var result = await _appointmentService.GetPendingFeedbackAsync(99);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPendingFeedbackAsync_WhenRepositoryThrows_ReturnsNull()
    {
        // Arrange
        _appointmentRepositoryMock
            .Setup(r => r.GetPendingFeedbackAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _appointmentService.GetPendingFeedbackAsync(1);

        // Assert
        result.Should().BeNull();
    }

    // ─── StoreFeedbackAsync ───────────────────────────────────────────────────

    [Fact]
    public async Task StoreFeedbackAsync_WhenExists_SetsFeedbackGivenAndReturnsTrue()
    {
        // Arrange
        var appointment = CreateAppointment(1, 3);
        _appointmentRepositoryMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointment);

        // Act
        var result = await _appointmentService.StoreFeedbackAsync(1);

        // Assert
        result.Should().BeTrue();
        appointment.FeedbackStatus.Should().Be(1);
        _appointmentRepositoryMock.Verify(r => r.UpdateAsync(appointment, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StoreFeedbackAsync_WhenNotFound_ReturnsFalse()
    {
        // Arrange
        _appointmentRepositoryMock
            .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Appointment?)null);

        // Act
        var result = await _appointmentService.StoreFeedbackAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task StoreFeedbackAsync_WhenRepositoryThrows_ReturnsFalse()
    {
        // Arrange
        _appointmentRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _appointmentService.StoreFeedbackAsync(1);

        // Assert
        result.Should().BeFalse();
    }

    // ─── GetPatientHistoryAsync ───────────────────────────────────────────────

    [Fact]
    public async Task GetPatientHistoryAsync_ReturnsOnlyCompletedAppointments()
    {
        // Arrange
        var appointments = new List<Appointment>
        {
            CreateAppointment(1, 3), // Completed
            CreateAppointment(2, 1), // Approved - should be filtered out
            CreateAppointment(3, 3)  // Completed
        };

        _appointmentRepositoryMock
            .Setup(r => r.GetByDoctorIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointments);

        // Act
        var result = await _appointmentService.GetPatientHistoryAsync(10);

        // Assert
        result.Should().HaveCount(2);
        result.All(a => a.AppointmentStatus == 3).Should().BeTrue();
    }

    [Fact]
    public async Task GetPatientHistoryAsync_WhenRepositoryThrows_ReturnsEmpty()
    {
        // Arrange
        _appointmentRepositoryMock
            .Setup(r => r.GetByDoctorIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _appointmentService.GetPatientHistoryAsync(1);

        // Assert
        result.Should().BeEmpty();
    }

    // ─── MapToDto (via public methods) ───────────────────────────────────────

    [Fact]
    public async Task GetCurrentAppointmentAsync_MapsAllFieldsCorrectly()
    {
        // Arrange
        var date = new DateTime(2024, 6, 15, 10, 30, 0);
        var appointment = new Appointment
        {
            AppointId = 42,
            DoctorId = 10,
            PatientId = 20,
            Date = date,
            AppointmentStatus = 1,
            BillAmount = 750.0,
            BillStatus = "Paid",
            Disease = "Hypertension",
            Progress = "Stable",
            Prescription = "Amlodipine 5mg",
            FeedbackStatus = 1,
            Doctor = new Doctor { DoctorId = 10, Name = "Dr. House" },
            Patient = new Patient { PatientId = 20, Name = "Patient Zero" }
        };

        _appointmentRepositoryMock
            .Setup(r => r.GetCurrentByPatientIdAsync(20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointment);

        // Act
        var result = await _appointmentService.GetCurrentAppointmentAsync(20);

        // Assert
        result.Should().NotBeNull();
        result!.AppointId.Should().Be(42);
        result.DoctorId.Should().Be(10);
        result.DoctorName.Should().Be("Dr. House");
        result.PatientId.Should().Be(20);
        result.PatientName.Should().Be("Patient Zero");
        result.Date.Should().Be(date);
        result.AppointmentStatus.Should().Be(1);
        result.BillAmount.Should().Be(750.0);
        result.BillStatus.Should().Be("Paid");
        result.Disease.Should().Be("Hypertension");
        result.Progress.Should().Be("Stable");
        result.Prescription.Should().Be("Amlodipine 5mg");
        result.FeedbackStatus.Should().Be(1);
    }
}
