using ClinicManagement.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace ClinicManagement.UnitTests.Domain;

/// <summary>
/// Unit tests for Domain Exceptions.
/// </summary>
public class DomainExceptionTests
{
    // ─── NotFoundException ────────────────────────────────────────────────────

    [Fact]
    public void NotFoundException_WithMessage_SetsMessage()
    {
        // Arrange & Act
        var ex = new NotFoundException("Entity not found");

        // Assert
        ex.Message.Should().Be("Entity not found");
    }

    [Fact]
    public void NotFoundException_WithEntityNameAndKey_FormatsMessage()
    {
        // Arrange & Act
        var ex = new NotFoundException("Doctor", 42);

        // Assert
        ex.Message.Should().Contain("Doctor");
        ex.Message.Should().Contain("42");
    }

    [Fact]
    public void NotFoundException_WithStringKey_FormatsMessage()
    {
        // Arrange & Act
        var ex = new NotFoundException("Patient", "john@example.com");

        // Assert
        ex.Message.Should().Contain("Patient");
        ex.Message.Should().Contain("john@example.com");
    }

    [Fact]
    public void NotFoundException_IsExceptionSubclass()
    {
        // Arrange & Act
        var ex = new NotFoundException("Test");

        // Assert
        ex.Should().BeAssignableTo<Exception>();
    }

    [Fact]
    public void NotFoundException_CanBeCaughtAsException()
    {
        // Arrange
        Action action = () => throw new NotFoundException("Not found");

        // Assert
        action.Should().Throw<NotFoundException>().WithMessage("Not found");
    }

    [Fact]
    public void NotFoundException_WithEntityAndKey_CanBeCaught()
    {
        // Arrange
        Action action = () => throw new NotFoundException("Appointment", 99);

        // Assert
        action.Should().Throw<NotFoundException>()
            .WithMessage("*Appointment*")
            .WithMessage("*99*");
    }

    // ─── ValidationException ──────────────────────────────────────────────────

    [Fact]
    public void ValidationException_WithMessage_SetsMessage()
    {
        // Arrange & Act
        var ex = new ValidationException("Validation failed");

        // Assert
        ex.Message.Should().Be("Validation failed");
    }

    [Fact]
    public void ValidationException_IsExceptionSubclass()
    {
        // Arrange & Act
        var ex = new ValidationException("Test");

        // Assert
        ex.Should().BeAssignableTo<Exception>();
    }

    [Fact]
    public void ValidationException_CanBeCaughtAsException()
    {
        // Arrange
        Action action = () => throw new ValidationException("Invalid input");

        // Assert
        action.Should().Throw<ValidationException>().WithMessage("Invalid input");
    }

    [Fact]
    public void ValidationException_WithEmptyMessage_SetsEmptyMessage()
    {
        // Arrange & Act
        var ex = new ValidationException(string.Empty);

        // Assert
        ex.Message.Should().Be(string.Empty);
    }

    // ─── DuplicateEntryException ──────────────────────────────────────────────

    [Fact]
    public void DuplicateEntryException_WithMessage_SetsMessage()
    {
        // Arrange & Act
        var ex = new DuplicateEntryException("Duplicate email");

        // Assert
        ex.Message.Should().Be("Duplicate email");
    }

    [Fact]
    public void DuplicateEntryException_IsExceptionSubclass()
    {
        // Arrange & Act
        var ex = new DuplicateEntryException("Test");

        // Assert
        ex.Should().BeAssignableTo<Exception>();
    }

    [Fact]
    public void DuplicateEntryException_CanBeCaughtAsException()
    {
        // Arrange
        Action action = () => throw new DuplicateEntryException("Email already exists");

        // Assert
        action.Should().Throw<DuplicateEntryException>().WithMessage("Email already exists");
    }

    [Fact]
    public void DuplicateEntryException_CanBeCaughtAsBaseException()
    {
        // Arrange
        Action action = () => throw new DuplicateEntryException("Duplicate");

        // Assert
        action.Should().Throw<Exception>();
    }

    // ─── Exception hierarchy ──────────────────────────────────────────────────

    [Fact]
    public void AllDomainExceptions_AreDistinctTypes()
    {
        // Arrange
        var notFound = new NotFoundException("test");
        var validation = new ValidationException("test");
        var duplicate = new DuplicateEntryException("test");

        // Assert
        notFound.GetType().Should().NotBe(validation.GetType());
        notFound.GetType().Should().NotBe(duplicate.GetType());
        validation.GetType().Should().NotBe(duplicate.GetType());
    }
}
