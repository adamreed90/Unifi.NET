using System.Text.Json.Serialization;

namespace Unifi.NET.Access.Models.AccessPolicies;

/// <summary>
/// Access policy response model.
/// </summary>
public sealed class AccessPolicyResponse
{
    /// <summary>
    /// Identity ID of the policy.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Name of the access policy.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Resources assigned to the policy.
    /// </summary>
    [JsonPropertyName("resources")]
    public List<AccessPolicyResource>? Resources { get; set; }

    /// <summary>
    /// Identity ID of the schedule.
    /// </summary>
    [JsonPropertyName("schedule_id")]
    public string? ScheduleId { get; set; }
}