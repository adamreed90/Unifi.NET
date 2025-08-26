using System.Text.Json.Serialization;

namespace Unifi.NET.Access.Models.SystemLogs;

/// <summary>
/// Request model for exporting system logs to CSV.
/// </summary>
public sealed class SystemLogExportRequest
{
    /// <summary>
    /// The topic of system logs to export.
    /// Valid values: critical, door_openings, updates, device_events, admin_activity, visitor
    /// </summary>
    [JsonPropertyName("topic")]
    public string Topic { get; set; } = "door_openings";

    /// <summary>
    /// Start time for log export (Unix timestamp in seconds).
    /// Required field.
    /// </summary>
    [JsonPropertyName("since")]
    public long Since { get; set; }

    /// <summary>
    /// End time for log export (Unix timestamp in seconds).
    /// Required field. Note: The since and until periods cannot exceed one month.
    /// </summary>
    [JsonPropertyName("until")]
    public long Until { get; set; }

    /// <summary>
    /// Identity ID of the actor (user, visitor, or device) to filter logs by.
    /// </summary>
    [JsonPropertyName("actor_id")]
    public string? ActorId { get; set; }
}