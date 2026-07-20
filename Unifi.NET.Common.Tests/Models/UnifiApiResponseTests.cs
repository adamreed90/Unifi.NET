using FluentAssertions;
using Unifi.NET.Common.Models;

namespace Unifi.NET.Common.Tests.Models;

public class UnifiApiResponseTests
{
    [Fact]
    public void IsSuccess_WhenCodeIsSuccess_ReturnsTrue()
    {
        // Arrange
        var response = new UnifiApiResponse<string>
        {
            Code = "SUCCESS",
            Message = "success",
            Data = "payload"
        };

        // Act & Assert
        response.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData("CODE_PARAMS_INVALID")]
    [InlineData("CODE_AUTH_FAILED")]
    [InlineData("")]
    public void IsSuccess_WhenCodeIsNotSuccess_ReturnsFalse(string code)
    {
        // Arrange
        var response = new UnifiApiResponse<string>
        {
            Code = code,
            Message = "failure"
        };

        // Act & Assert
        response.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void Data_WhenNotSet_DefaultsToNullForReferenceType()
    {
        // Arrange
        var response = new UnifiApiResponse<string>
        {
            Code = "SUCCESS",
            Message = "success"
        };

        // Act & Assert
        response.Data.Should().BeNull();
    }

    [Fact]
    public void Data_CarriesTypedPayload()
    {
        // Arrange
        var response = new UnifiApiResponse<List<int>>
        {
            Code = "SUCCESS",
            Message = "success",
            Data = [1, 2, 3]
        };

        // Act & Assert
        response.Data.Should().BeEquivalentTo([1, 2, 3]);
        response.IsSuccess.Should().BeTrue();
    }
}
