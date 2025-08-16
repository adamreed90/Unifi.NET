using System.Text.Json.Serialization;

namespace Unifi.NET.Access.Models.Doors;

/// <summary>
/// Request to set a temporary door locking rule.
/// </summary>
public sealed class SetDoorLockingRuleRequest
{
    /// <summary>
    /// Type of locking rule.
    /// </summary>
    [JsonPropertyName("type")]
    public required string Type { get; set; }

    /// <summary>
    /// Duration of the rule in seconds (optional).
    /// </summary>
    [JsonPropertyName("duration")]
    public int? Duration { get; set; }
}