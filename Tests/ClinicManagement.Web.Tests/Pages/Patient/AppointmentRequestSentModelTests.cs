using System;
using System.Threading;
using System.Threading.Tasks;
using ClinicManagement.Web.Pages.Patient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace ClinicManagement.Web.Tests.Pages.Patient;

public class AppointmentRequestSentModelTests
{
    [Fact]
    public void Constructor_CreatesInstance()
    {
        // Arrange & Act
        var model = new AppointmentRequestSentModel();

        // Assert
        Assert.NotNull(model);
    }

    [Fact]
    public void OnGet_DoesNotThrow()
    {
        // Arrange
        var model = new AppointmentRequestSentModel();
        var httpContext = new DefaultHttpContext();
        var session = new MockSession();
        httpContext.Session = session;
        var actionContext = new ActionContext(
            httpContext,
            new Microsoft.AspNetCore.Routing.RouteData(),
            new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
        model.PageContext = new PageContext(actionContext);

        // Act & Assert (should not throw)
        model.OnGet();
    }

    [Fact]
    public void OnGet_ReturnsVoid()
    {
        // Arrange
        var model = new AppointmentRequestSentModel();
        var httpContext = new DefaultHttpContext();
        httpContext.Session = new MockSession();
        var actionContext = new ActionContext(
            httpContext,
            new Microsoft.AspNetCore.Routing.RouteData(),
            new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
        model.PageContext = new PageContext(actionContext);

        // Act
        var exception = Record.Exception(() => model.OnGet());

        // Assert
        Assert.Null(exception);
    }
}
