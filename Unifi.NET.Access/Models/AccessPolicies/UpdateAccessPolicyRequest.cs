using System.Text.Json.Serialization;

namespace Unifi.NET.Access.Models.AccessPolicies;

/// <summary>
/// Request to update an existing access policy.
/// </summary>
public sealed class UpdateAccessPolicyRequest
{
    /// <summary>
    /// Name of the access policy.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Resources to assign to the access policy.
    /// </summary>
    [JsonPropertyName("resource")]
    public List<AccessPolicyResource>? Resources { get; set; }

    /// <summary>
    /// Identity ID of the schedule.
    /// </summary>
    [JsonPropertyName("schedule_id")]
    public string? ScheduleId { get; set; }
}