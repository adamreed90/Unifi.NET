using System.Text.Json.Serialization;

namespace Unifi.NET.Access.Models.Doors;

/// <summary>
/// Request to set door emergency status.
/// </summary>
public sealed class SetDoorEmergencyStatusRequest
{
    /// <summary>
    /// Emergency status (true for emergency mode, false for normal mode).
    /// </summary>
    [JsonPropertyName("emergency")]
    public required bool Emergency { get; set; }
}