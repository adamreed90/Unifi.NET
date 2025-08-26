using System.Text.Json.Serialization;

namespace Unifi.NET.Access.Models.Devices;

/// <summary>
/// Device response model.
/// </summary>
public sealed class DeviceResponse
{
    /// <summary>
    /// Identity ID of the device.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Name of the device.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Type of the device (UAH, UDA-LITE, UA-G2-PRO, etc.).
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Alias of the device.
    /// </summary>
    [JsonPropertyName("alias")]
    public string Alias { get; set; } = string.Empty;

    /// <summary>
    /// The ID of the connected UA Hub (parent device).
    /// Empty string if this is a hub or standalone device.
    /// </summary>
    [JsonPropertyName("connected_uah_id")]
    public string ConnectedUahId { get; set; } = string.Empty;

    /// <summary>
    /// The location ID for grouping devices by physical location.
    /// </summary>
    [JsonPropertyName("location_id")]
    public string LocationId { get; set; } = string.Empty;
}