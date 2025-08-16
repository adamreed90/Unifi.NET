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
}