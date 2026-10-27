using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClinicManagement.Application.Services;
using ClinicManagement.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;

namespace ClinicManagement.Web.Tests.Helpers;

/// <summary>Shared test helpers for creating mocks and page contexts.</summary>
public static class TestHelpers
{
    public static Mock<PatientService> CreatePatientServiceMock()
    {
        var patientRepoMock = new Mock<IPatientRepository>();
        var appointmentRepoMock = new Mock<IAppointmentRepository>();
        var doctorRepoMock = new Mock<IDoctorRepository>();
        var deptRepoMock = new Mock<IDepartmentRepository>();
        var loggerMock = new Mock<ILogger<PatientService>>();
        return new Mock<PatientService>(
            patientRepoMock.Object,
            appointmentRepoMock.Object,
            doctorRepoMock.Object,
            deptRepoMock.Object,
            loggerMock.Object);
    }

    public static Mock<DoctorService> CreateDoctorServiceMock()
    {
        var doctorRepoMock = new Mock<IDoctorRepository>();
        var appointmentRepoMock = new Mock<IAppointmentRepository>();
        var loggerMock = new Mock<ILogger<DoctorService>>();
        return new Mock<DoctorService>(
            doctorRepoMock.Object,
            appointmentRepoMock.Object,
            loggerMock.Object);
    }

    public static Mock<AdminService> CreateAdminServiceMock()
    {
        var doctorRepoMock = new Mock<IDoctorRepository>();
        var staffRepoMock = new Mock<IStaffRepository>();
        var patientRepoMock = new Mock<IPatientRepository>();
        var appointmentRepoMock = new Mock<IAppointmentRepository>();
        var deptRepoMock = new Mock<IDepartmentRepository>();
        var loginRepoMock = new Mock<ILoginRepository>();
        var loggerMock = new Mock<ILogger<AdminService>>();
        return new Mock<AdminService>(
            doctorRepoMock.Object,
            staffRepoMock.Object,
            patientRepoMock.Object,
            appointmentRepoMock.Object,
            deptRepoMock.Object,
            loginRepoMock.Object,
            loggerMock.Object);
    }

    public static Mock<AuthService> CreateAuthServiceMock()
    {
        var loginRepoMock = new Mock<ILoginRepository>();
        var patientRepoMock = new Mock<IPatientRepository>();
        var loggerMock = new Mock<ILogger<AuthService>>();
        return new Mock<AuthService>(
            loginRepoMock.Object,
            patientRepoMock.Object,
            loggerMock.Object);
    }

    public static DefaultHttpContext CreateHttpContext(int? userId = null, int? userType = null)
    {
        var context = new DefaultHttpContext();
        var session = new ClinicManagement.Web.Tests.Pages.MockSession();
        if (userId.HasValue)
            session.SetInt32("UserId", userId.Value);
        if (userType.HasValue)
            session.SetInt32("UserType", userType.Value);
        context.Session = session;
        return context;
    }

    public static PageContext CreatePageContext(HttpContext httpContext)
    {
        var actionContext = new ActionContext(
            httpContext,
            new Microsoft.AspNetCore.Routing.RouteData(),
            new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
        return new PageContext(actionContext);
    }
}
