using System.Text.Json.Serialization;
using Unifi.NET.Access.Models;
using Unifi.NET.Access.Models.UserGroups;

namespace Unifi.NET.Access.Serialization.Contexts;

/// <summary>
/// JSON serialization context for user group types.
/// </summary>
[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    GenerationMode = JsonSourceGenerationMode.Metadata)]
// Request/Response types
[JsonSerializable(typeof(UserGroupRequest))]
[JsonSerializable(typeof(UserGroupResponse))]
[JsonSerializable(typeof(AssignUsersToGroupRequest))]
[JsonSerializable(typeof(ImportUserGroupsRequest))]
[JsonSerializable(typeof(ImportUserGroupsResponse))]
[JsonSerializable(typeof(List<UserGroupResponse>))]
[JsonSerializable(typeof(List<ImportUserGroupsResponse>))]
// Paginated response types
[JsonSerializable(typeof(PaginatedResponse<List<UserGroupResponse>>))]
// API wrapper types
[JsonSerializable(typeof(UnifiApiResponse<UserGroupResponse>))]
[JsonSerializable(typeof(UnifiApiResponse<List<UserGroupResponse>>))]
[JsonSerializable(typeof(UnifiApiResponse<List<ImportUserGroupsResponse>>))]
internal partial class UserGroupJsonContext : JsonSerializerContext
{
}