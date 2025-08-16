using System.Text.Json.Serialization;

namespace Unifi.NET.Access.Models.UserGroups;

/// <summary>
/// Request to create or update a user group.
/// </summary>
public sealed class UserGroupRequest
{
    /// <summary>
    /// Name of the user group.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>
    /// Description of the user group.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Parent group ID if this is a subgroup.
    /// </summary>
    [JsonPropertyName("parent_id")]
    public string? ParentId { get; set; }

    /// <summary>
    /// Door group IDs associated with this user group.
    /// </summary>
    [JsonPropertyName("door_group_ids")]
    public List<string>? DoorGroupIds { get; set; }

    /// <summary>
    /// Access policy schedule ID.
    /// </summary>
    [JsonPropertyName("schedule_id")]
    public string? ScheduleId { get; set; }
}