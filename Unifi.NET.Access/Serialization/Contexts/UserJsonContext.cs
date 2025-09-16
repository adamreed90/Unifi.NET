using System.Text.Json.Serialization;
using Unifi.NET.Access.Models;
using Unifi.NET.Access.Models.Users;

namespace Unifi.NET.Access.Serialization.Contexts;

/// <summary>
/// JSON serialization context for user-related types.
/// </summary>
[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    GenerationMode = JsonSourceGenerationMode.Metadata)]
// Request/Response types
[JsonSerializable(typeof(CreateUserRequest))]
[JsonSerializable(typeof(UpdateUserRequest))]
[JsonSerializable(typeof(UserResponse))]
[JsonSerializable(typeof(AccessPolicyInfo))]
[JsonSerializable(typeof(List<UserResponse>))]
[JsonSerializable(typeof(List<AccessPolicyInfo>))]
// Paginated response types
[JsonSerializable(typeof(PaginatedResponse<List<UserResponse>>))]
// API wrapper types
[JsonSerializable(typeof(UnifiApiResponse<UserResponse>))]
[JsonSerializable(typeof(UnifiApiResponse<List<UserResponse>>))]
internal partial class UserJsonContext : JsonSerializerContext
{
}