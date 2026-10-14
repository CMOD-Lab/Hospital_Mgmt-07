using ClinicManagement.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace ClinicManagement.UnitTests.Domain;

/// <summary>
/// Unit tests for Domain Enums.
/// </summary>
public class DomainEnumTests
{
    // ─── UserType ─────────────────────────────────────────────────────────────

    [Fact]
    public void UserType_Patient_HasValueOne()
    {
        ((int)UserType.Patient).Should().Be(1);
    }

    [Fact]
    public void UserType_Doctor_HasValueTwo()
    {
        ((int)UserType.Doctor).Should().Be(2);
    }

    [Fact]
    public void UserType_Admin_HasValueThree()
    {
        ((int)UserType.Admin).Should().Be(3);
    }

    [Theory]
    [InlineData(1, UserType.Patient)]
    [InlineData(2, UserType.Doctor)]
    [InlineData(3, UserType.Admin)]
    public void UserType_CastFromInt_ReturnsCorrectEnum(int value, UserType expected)
    {
        ((UserType)value).Should().Be(expected);
    }

    // ─── AppointmentStatus ────────────────────────────────────────────────────

    [Fact]
    public void AppointmentStatus_Approved_HasValueOne()
    {
        ((int)AppointmentStatus.Approved).Should().Be(1);
    }

    [Fact]
    public void AppointmentStatus_Pending_HasValueTwo()
    {
        ((int)AppointmentStatus.Pending).Should().Be(2);
    }

    [Fact]
    public void AppointmentStatus_Completed_HasValueThree()
    {
        ((int)AppointmentStatus.Completed).Should().Be(3);
    }

    [Fact]
    public void AppointmentStatus_Rejected_HasValueFour()
    {
        ((int)AppointmentStatus.Rejected).Should().Be(4);
    }

    [Theory]
    [InlineData(1, AppointmentStatus.Approved)]
    [InlineData(2, AppointmentStatus.Pending)]
    [InlineData(3, AppointmentStatus.Completed)]
    [InlineData(4, AppointmentStatus.Rejected)]
    public void AppointmentStatus_CastFromInt_ReturnsCorrectEnum(int value, AppointmentStatus expected)
    {
        ((AppointmentStatus)value).Should().Be(expected);
    }

    // ─── NotificationStatus ───────────────────────────────────────────────────

    [Fact]
    public void NotificationStatus_Seen_HasValueOne()
    {
        ((int)NotificationStatus.Seen).Should().Be(1);
    }

    [Fact]
    public void NotificationStatus_Unseen_HasValueTwo()
    {
        ((int)NotificationStatus.Unseen).Should().Be(2);
    }

    [Theory]
    [InlineData(1, NotificationStatus.Seen)]
    [InlineData(2, NotificationStatus.Unseen)]
    public void NotificationStatus_CastFromInt_ReturnsCorrectEnum(int value, NotificationStatus expected)
    {
        ((NotificationStatus)value).Should().Be(expected);
    }

    // ─── FeedbackStatus ───────────────────────────────────────────────────────

    [Fact]
    public void FeedbackStatus_Given_HasValueOne()
    {
        ((int)FeedbackStatus.Given).Should().Be(1);
    }

    [Fact]
    public void FeedbackStatus_Pending_HasValueTwo()
    {
        ((int)FeedbackStatus.Pending).Should().Be(2);
    }

    [Theory]
    [InlineData(1, FeedbackStatus.Given)]
    [InlineData(2, FeedbackStatus.Pending)]
    public void FeedbackStatus_CastFromInt_ReturnsCorrectEnum(int value, FeedbackStatus expected)
    {
        ((FeedbackStatus)value).Should().Be(expected);
    }

    // ─── DoctorStatus ─────────────────────────────────────────────────────────

    [Fact]
    public void DoctorStatus_Left_HasValueZero()
    {
        ((int)DoctorStatus.Left).Should().Be(0);
    }

    [Fact]
    public void DoctorStatus_Present_HasValueOne()
    {
        ((int)DoctorStatus.Present).Should().Be(1);
    }

    [Theory]
    [InlineData(0, DoctorStatus.Left)]
    [InlineData(1, DoctorStatus.Present)]
    public void DoctorStatus_CastFromInt_ReturnsCorrectEnum(int value, DoctorStatus expected)
    {
        ((DoctorStatus)value).Should().Be(expected);
    }

    // ─── Enum names ───────────────────────────────────────────────────────────

    [Fact]
    public void UserType_EnumNames_AreCorrect()
    {
        Enum.GetNames(typeof(UserType)).Should().Contain(new[] { "Patient", "Doctor", "Admin" });
    }

    [Fact]
    public void AppointmentStatus_EnumNames_AreCorrect()
    {
        Enum.GetNames(typeof(AppointmentStatus)).Should().Contain(new[] { "Approved", "Pending", "Completed", "Rejected" });
    }

    [Fact]
    public void NotificationStatus_EnumNames_AreCorrect()
    {
        Enum.GetNames(typeof(NotificationStatus)).Should().Contain(new[] { "Seen", "Unseen" });
    }

    [Fact]
    public void FeedbackStatus_EnumNames_AreCorrect()
    {
        Enum.GetNames(typeof(FeedbackStatus)).Should().Contain(new[] { "Given", "Pending" });
    }

    [Fact]
    public void DoctorStatus_EnumNames_AreCorrect()
    {
        Enum.GetNames(typeof(DoctorStatus)).Should().Contain(new[] { "Left", "Present" });
    }
}
