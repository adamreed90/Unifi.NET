using System.Text.Json.Serialization;

namespace Unifi.NET.Access.Models.UserGroups;

/// <summary>
/// Request to assign users to a user group.
/// </summary>
public sealed class AssignUsersToGroupRequest
{
    /// <summary>
    /// User IDs to assign to the group.
    /// </summary>
    [JsonPropertyName("user_ids")]
    public required List<string> UserIds { get; set; }
}