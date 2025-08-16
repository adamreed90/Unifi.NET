using System.Text.Json.Serialization;

namespace Unifi.NET.Access.Models.Doors;

/// <summary>
/// Door emergency status response model.
/// </summary>
public sealed class DoorEmergencyStatusResponse
{
    /// <summary>
    /// Emergency status.
    /// </summary>
    [JsonPropertyName("emergency")]
    public bool Emergency { get; set; }
}