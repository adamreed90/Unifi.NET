using System.Text.Json.Serialization;

namespace Unifi.NET.Access.Models.Doors;

/// <summary>
/// Door response model.
/// </summary>
public sealed class DoorResponse
{
    /// <summary>
    /// Identity ID of the door.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Name of the door.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Full name of the door.
    /// </summary>
    [JsonPropertyName("full_name")]
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Identity ID of the floor.
    /// </summary>
    [JsonPropertyName("floor_id")]
    public string? FloorId { get; set; }

    /// <summary>
    /// Type of the door.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the door has bound to a hub device.
    /// </summary>
    [JsonPropertyName("is_bind_hub")]
    public bool IsBindHub { get; set; }

    /// <summary>
    /// Door lock status (lock or unlock).
    /// </summary>
    [JsonPropertyName("door_lock_relay_status")]
    public string? DoorLockRelayStatus { get; set; }

    /// <summary>
    /// Door position status (open or close). Null means no device is connected.
    /// </summary>
    [JsonPropertyName("door_position_status")]
    public string? DoorPositionStatus { get; set; }
}