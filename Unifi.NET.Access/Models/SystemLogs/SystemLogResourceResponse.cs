using System.Text.Json.Serialization;

namespace Unifi.NET.Access.Models.SystemLogs;

/// <summary>
/// Response model for system log resource information.
/// </summary>
public sealed class SystemLogResourceResponse
{
    /// <summary>
    /// List of resources referenced in system logs.
    /// </summary>
    [JsonPropertyName("data")]
    public List<SystemLogResource> Data { get; set; } = new();
}

/// <summary>
/// Resource information referenced in system logs.
/// </summary>
public sealed class SystemLogResource
{
    /// <summary>
    /// Resource identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Display name of the resource.
    /// </summary>
    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Alternate identifier.
    /// </summary>
    [JsonPropertyName("alternate_id")]
    public string? AlternateId { get; set; }

    /// <summary>
    /// Alternate name.
    /// </summary>
    [JsonPropertyName("alternate_name")]
    public string? AlternateName { get; set; }
}