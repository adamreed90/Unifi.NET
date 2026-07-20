using FluentAssertions;
using Unifi.NET.Common.Exceptions;

namespace Unifi.NET.Common.Tests.Exceptions;

public class UnifiExceptionTests
{
    [Fact]
    public void Constructor_WithMessageOnly_SetsMessageAndLeavesErrorInfoNull()
    {
        // Arrange & Act
        var exception = new UnifiException("Something went wrong");

        // Assert
        exception.Message.Should().Be("Something went wrong");
        exception.ErrorCode.Should().BeNull();
        exception.StatusCode.Should().BeNull();
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithInnerException_PreservesInnerException()
    {
        // Arrange
        var inner = new InvalidOperationException("root cause");

        // Act
        var exception = new UnifiException("wrapped failure", inner);

        // Assert
        exception.Message.Should().Be("wrapped failure");
        exception.InnerException.Should().BeSameAs(inner);
        exception.ErrorCode.Should().BeNull();
        exception.StatusCode.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithErrorCodeAndStatusCode_PopulatesErrorInfo()
    {
        // Arrange & Act
        var exception = new UnifiException("Invalid parameters", "CODE_PARAMS_INVALID", 400);

        // Assert
        exception.Message.Should().Be("Invalid parameters");
        exception.ErrorCode.Should().Be("CODE_PARAMS_INVALID");
        exception.StatusCode.Should().Be(400);
    }

    [Fact]
    public void UnifiAuthenticationException_IsAUnifiException()
    {
        // Arrange & Act
        var exception = new UnifiAuthenticationException("Auth failed", "CODE_AUTH_FAILED", 401);

        // Assert
        exception.Should().BeAssignableTo<UnifiException>();
        exception.ErrorCode.Should().Be("CODE_AUTH_FAILED");
        exception.StatusCode.Should().Be(401);
        exception.Message.Should().Be("Auth failed");
    }

    [Fact]
    public void UnifiNotFoundException_IsAUnifiException()
    {
        // Arrange & Act
        var exception = new UnifiNotFoundException("Resource not found", "CODE_RESOURCE_NOT_FOUND", 404);

        // Assert
        exception.Should().BeAssignableTo<UnifiException>();
        exception.ErrorCode.Should().Be("CODE_RESOURCE_NOT_FOUND");
        exception.StatusCode.Should().Be(404);
    }

    [Fact]
    public void UnifiAuthenticationException_MessageOnlyConstructor_LeavesErrorInfoNull()
    {
        // Arrange & Act
        var exception = new UnifiAuthenticationException("Auth failed");

        // Assert
        exception.Message.Should().Be("Auth failed");
        exception.ErrorCode.Should().BeNull();
        exception.StatusCode.Should().BeNull();
    }

    [Fact]
    public void UnifiNotFoundException_MessageOnlyConstructor_LeavesErrorInfoNull()
    {
        // Arrange & Act
        var exception = new UnifiNotFoundException("Not found");

        // Assert
        exception.Message.Should().Be("Not found");
        exception.ErrorCode.Should().BeNull();
        exception.StatusCode.Should().BeNull();
    }
}
