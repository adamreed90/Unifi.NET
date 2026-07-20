using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using FluentAssertions;
using Unifi.NET.Access.Models;
using Unifi.NET.Access.Models.Doors;
using Unifi.NET.Access.Models.Users;
using Unifi.NET.Access.Serialization;

namespace Unifi.NET.Access.Tests.Serialization;

/// <summary>
/// Verifies that representative models round-trip through the combined
/// <see cref="UnifiAccessJsonContext"/> resolver, proving Native AOT
/// source-generated serialization coverage across service-specific contexts.
/// </summary>
public class UnifiAccessJsonContextTests
{
    private readonly JsonSerializerOptions _options = UnifiAccessJsonContext.CreateOptions();

    [Fact]
    public void CreateOptions_UsesCombinedResolverFromAllContexts()
    {
        // Act & Assert
        _options.TypeInfoResolver.Should().BeSameAs(UnifiAccessJsonContext.Combined);
    }

    [Fact]
    public void UserResponse_RoundTripsThroughCombinedResolver()
    {
        // Arrange - snake_case wire format as returned by the UniFi Access API,
        // exercising UserJsonContext (registered in the combined resolver).
        const string json = """
            {
                "id": "usr-123",
                "first_name": "Ada",
                "last_name": "Lovelace",
                "full_name": "Ada Lovelace",
                "user_email": "ada@example.com",
                "status": "ACTIVE",
                "access_policy_ids": ["policy-1", "policy-2"]
            }
            """;

        var typeInfo = (JsonTypeInfo<UserResponse>)_options.GetTypeInfo(typeof(UserResponse));

        // Act
        var user = JsonSerializer.Deserialize(json, typeInfo);
        user.Should().NotBeNull();
        var reserialized = JsonSerializer.Serialize(user!, typeInfo);
        var roundTripped = JsonSerializer.Deserialize(reserialized, typeInfo);

        // Assert
        user!.Id.Should().Be("usr-123");
        user.FirstName.Should().Be("Ada");
        user.LastName.Should().Be("Lovelace");
        user.UserEmail.Should().Be("ada@example.com");
        user.Status.Should().Be("ACTIVE");
        user.AccessPolicyIds.Should().Equal("policy-1", "policy-2");

        roundTripped.Should().BeEquivalentTo(user);
    }

    [Fact]
    public void UnifiApiResponse_OfUserResponse_RoundTripsThroughCombinedResolver()
    {
        // Arrange - full API envelope, exercising the UnifiApiResponse<UserResponse>
        // wrapper type registered by UserJsonContext.
        const string json = """
            {
                "code": "SUCCESS",
                "msg": "success",
                "data": {
                    "id": "usr-456",
                    "first_name": "Grace",
                    "last_name": "Hopper"
                }
            }
            """;

        var typeInfo = (JsonTypeInfo<UnifiApiResponse<UserResponse>>)
            _options.GetTypeInfo(typeof(UnifiApiResponse<UserResponse>));

        // Act
        var apiResponse = JsonSerializer.Deserialize(json, typeInfo);

        // Assert
        apiResponse.Should().NotBeNull();
        apiResponse!.Code.Should().Be("SUCCESS");
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data!.FirstName.Should().Be("Grace");
        apiResponse.Data.LastName.Should().Be("Hopper");
    }

    [Fact]
    public void DoorResponse_RoundTripsThroughCombinedResolver()
    {
        // Arrange - exercises DoorJsonContext (a different service-specific
        // context) via the same combined resolver used above for UserResponse.
        const string json = """
            {
                "id": "door-789",
                "name": "Front Door",
                "full_name": "Main Building / Front Door",
                "type": "door",
                "is_bind_hub": true,
                "door_lock_relay_status": "lock",
                "door_position_status": "close"
            }
            """;

        var typeInfo = (JsonTypeInfo<DoorResponse>)_options.GetTypeInfo(typeof(DoorResponse));

        // Act
        var door = JsonSerializer.Deserialize(json, typeInfo);
        door.Should().NotBeNull();
        var reserialized = JsonSerializer.Serialize(door!, typeInfo);
        var roundTripped = JsonSerializer.Deserialize(reserialized, typeInfo);

        // Assert
        door!.Id.Should().Be("door-789");
        door.Name.Should().Be("Front Door");
        door.IsBindHub.Should().BeTrue();
        door.DoorLockRelayStatus.Should().Be("lock");
        door.DoorPositionStatus.Should().Be("close");

        roundTripped.Should().BeEquivalentTo(door);
    }

    [Fact]
    public void UnifiApiResponse_OfDoorResponseList_RoundTripsThroughCombinedResolver()
    {
        // Arrange - exercises the List<DoorResponse>-wrapped envelope type,
        // as returned by the "list doors" endpoint.
        const string json = """
            {
                "code": "SUCCESS",
                "msg": "success",
                "data": [
                    { "id": "door-1", "name": "Lobby", "full_name": "Lobby", "type": "door", "is_bind_hub": false },
                    { "id": "door-2", "name": "Garage", "full_name": "Garage", "type": "door", "is_bind_hub": true }
                ]
            }
            """;

        var typeInfo = (JsonTypeInfo<UnifiApiResponse<List<DoorResponse>>>)
            _options.GetTypeInfo(typeof(UnifiApiResponse<List<DoorResponse>>));

        // Act
        var apiResponse = JsonSerializer.Deserialize(json, typeInfo);

        // Assert
        apiResponse.Should().NotBeNull();
        apiResponse!.Data.Should().HaveCount(2);
        apiResponse.Data![0].Id.Should().Be("door-1");
        apiResponse.Data[1].IsBindHub.Should().BeTrue();
    }

    [Fact]
    public void UserResponse_Serialize_OmitsNullOptionalPropertiesUsingCamelCaseNames()
    {
        // Arrange - the source-generated contexts configure camelCase naming
        // and omit nulls on write; verify that policy holds through the
        // combined resolver too.
        var user = new UserResponse
        {
            Id = "usr-1",
            FirstName = "Jane",
            LastName = "Doe"
        };

        var typeInfo = (JsonTypeInfo<UserResponse>)_options.GetTypeInfo(typeof(UserResponse));

        // Act
        var json = JsonSerializer.Serialize(user, typeInfo);

        // Assert - property names use the explicit [JsonPropertyName] (snake_case)
        // values, and unset nullable members are absent from the payload.
        json.Should().Contain("\"id\":\"usr-1\"");
        json.Should().Contain("\"first_name\":\"Jane\"");
        json.Should().NotContain("full_name");
        json.Should().NotContain("nfc_cards");
    }
}
