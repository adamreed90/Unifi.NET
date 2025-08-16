using System.Text.Json.Serialization;

namespace Unifi.NET.Access.Models.UserGroups;

/// <summary>
/// Represents a user group in UniFi Access.
/// </summary>
public sealed class UserGroupResponse
{
    /// <summary>
    /// Unique identifier of the user group.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Name of the user group.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the user group.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Number of users in this group.
    /// </summary>
    [JsonPropertyName("user_count")]
    public int UserCount { get; set; }

    /// <summary>
    /// Parent group ID if this is a subgroup.
    /// </summary>
    [JsonPropertyName("parent_id")]
    public string? ParentId { get; set; }

    /// <summary>
    /// Parent group name if this is a subgroup.
    /// </summary>
    [JsonPropertyName("parent_name")]
    public string? ParentName { get; set; }

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

    /// <summary>
    /// Whether this is a system group.
    /// </summary>
    [JsonPropertyName("is_system")]
    public bool IsSystem { get; set; }

    /// <summary>
    /// Creation timestamp.
    /// </summary>
    [JsonPropertyName("create_time")]
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// Last update timestamp.
    /// </summary>
    [JsonPropertyName("update_time")]
    public DateTime? UpdateTime { get; set; }
}