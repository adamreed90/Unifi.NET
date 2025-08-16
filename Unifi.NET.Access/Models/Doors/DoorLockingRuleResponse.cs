using System.Text.Json.Serialization;

namespace Unifi.NET.Access.Models.Doors;

/// <summary>
/// Door locking rule response model.
/// </summary>
public sealed class DoorLockingRuleResponse
{
    /// <summary>
    /// Type of locking rule.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Duration of the rule in seconds.
    /// </summary>
    [JsonPropertyName("duration")]
    public int? Duration { get; set; }

    /// <summary>
    /// Expiration time of the rule.
    /// </summary>
    [JsonPropertyName("expires_at")]
    public long? ExpiresAt { get; set; }
}