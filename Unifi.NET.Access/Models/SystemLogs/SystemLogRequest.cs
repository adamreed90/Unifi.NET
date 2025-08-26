using System.Text.Json.Serialization;

namespace Unifi.NET.Access.Models.SystemLogs;

/// <summary>
/// Request model for fetching system logs.
/// </summary>
public sealed class SystemLogRequest
{
    /// <summary>
    /// The topic of system logs to fetch.
    /// Valid values: critical, door_openings, updates, device_events, admin_activity, visitor
    /// </summary>
    [JsonPropertyName("topic")]
    public string Topic { get; set; } = "door_openings";

    /// <summary>
    /// Start time for log fetching (Unix timestamp in seconds).
    /// </summary>
    [JsonPropertyName("since")]
    public long? Since { get; set; }

    /// <summary>
    /// End time for log fetching (Unix timestamp in seconds).
    /// </summary>
    [JsonPropertyName("until")]
    public long? Until { get; set; }

    /// <summary>
    /// Identity ID of the actor (user, visitor, or device) to filter logs by.
    /// </summary>
    [JsonPropertyName("actor_id")]
    public string? ActorId { get; set; }
}

/// <summary>
/// System log topic types.
/// </summary>
public static class SystemLogTopic
{
    public const string Critical = "critical";
    public const string DoorOpenings = "door_openings";
    public const string Updates = "updates";
    public const string DeviceEvents = "device_events";
    public const string AdminActivity = "admin_activity";
    public const string Visitor = "visitor";
}