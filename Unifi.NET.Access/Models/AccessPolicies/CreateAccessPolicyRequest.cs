using System.Text.Json.Serialization;

namespace Unifi.NET.Access.Models.AccessPolicies;

/// <summary>
/// Request to create a new access policy.
/// </summary>
public sealed class CreateAccessPolicyRequest
{
    /// <summary>
    /// Name of the access policy.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>
    /// Resources to assign to the access policy.
    /// </summary>
    [JsonPropertyName("resource")]
    public List<AccessPolicyResource>? Resources { get; set; }

    /// <summary>
    /// Identity ID of the schedule.
    /// </summary>
    [JsonPropertyName("schedule_id")]
    public required string ScheduleId { get; set; }
}

/// <summary>
/// Access policy resource.
/// </summary>
public sealed class AccessPolicyResource
{
    /// <summary>
    /// Resource ID (door or door group ID).
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    /// <summary>
    /// Resource type (door or door_group).
    /// </summary>
    [JsonPropertyName("type")]
    public required string Type { get; set; }
}