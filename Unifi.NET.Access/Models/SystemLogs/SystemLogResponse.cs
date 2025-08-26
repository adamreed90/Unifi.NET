using System.Text.Json.Serialization;

namespace Unifi.NET.Access.Models.SystemLogs;

/// <summary>
/// Response model for system logs query.
/// </summary>
public sealed class SystemLogResponse
{
    /// <summary>
    /// List of log entries.
    /// </summary>
    [JsonPropertyName("hits")]
    public List<SystemLogEntry> Hits { get; set; } = new();

    /// <summary>
    /// Pagination information.
    /// </summary>
    [JsonPropertyName("pagination")]
    public SystemLogPagination? Pagination { get; set; }
    
    /// <summary>
    /// Gets the current page number from pagination.
    /// </summary>
    [JsonIgnore]
    public int Page => Pagination?.PageNum ?? 1;

    /// <summary>
    /// Gets the total number of log entries from pagination.
    /// </summary>
    [JsonIgnore]
    public int Total => Pagination?.Total ?? 0;
}

/// <summary>
/// Pagination information for system logs.
/// </summary>
public sealed class SystemLogPagination
{
    /// <summary>
    /// Current page number.
    /// </summary>
    [JsonPropertyName("page_num")]
    public int PageNum { get; set; }

    /// <summary>
    /// Page size.
    /// </summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of records.
    /// </summary>
    [JsonPropertyName("total")]
    public int Total { get; set; }
}

/// <summary>
/// Individual system log entry.
/// </summary>
public sealed class SystemLogEntry
{
    /// <summary>
    /// Timestamp of the log entry in ISO 8601 format.
    /// </summary>
    [JsonPropertyName("@timestamp")]
    public string Timestamp { get; set; } = string.Empty;

    /// <summary>
    /// Unique identifier for the log entry.
    /// </summary>
    [JsonPropertyName("_id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Source data of the log entry.
    /// </summary>
    [JsonPropertyName("_source")]
    public SystemLogSource Source { get; set; } = new();

    /// <summary>
    /// Tag indicating the log category.
    /// </summary>
    [JsonPropertyName("tag")]
    public string Tag { get; set; } = string.Empty;
}

/// <summary>
/// Source data within a system log entry.
/// </summary>
public sealed class SystemLogSource
{
    /// <summary>
    /// Information about the actor who triggered the event.
    /// </summary>
    [JsonPropertyName("actor")]
    public SystemLogActor Actor { get; set; } = new();

    /// <summary>
    /// Authentication details for the access attempt.
    /// </summary>
    [JsonPropertyName("authentication")]
    public SystemLogAuthentication? Authentication { get; set; }

    /// <summary>
    /// Event details.
    /// </summary>
    [JsonPropertyName("event")]
    public SystemLogEvent Event { get; set; } = new();

    /// <summary>
    /// Target resources affected by the event.
    /// </summary>
    [JsonPropertyName("target")]
    public List<SystemLogTarget>? Target { get; set; }
}

/// <summary>
/// Actor information in system log.
/// </summary>
public sealed class SystemLogActor
{
    /// <summary>
    /// Actor's unique identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Actor type (user, visitor, device).
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Display name of the actor.
    /// </summary>
    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Alternate identifier (e.g., employee number).
    /// </summary>
    [JsonPropertyName("alternate_id")]
    public string? AlternateId { get; set; }

    /// <summary>
    /// Alternate name.
    /// </summary>
    [JsonPropertyName("alternate_name")]
    public string? AlternateName { get; set; }
}

/// <summary>
/// Authentication information for access attempts.
/// </summary>
public sealed class SystemLogAuthentication
{
    /// <summary>
    /// Credential provider used (NFC, PIN, TouchPass, etc.).
    /// </summary>
    [JsonPropertyName("credential_provider")]
    public string CredentialProvider { get; set; } = string.Empty;

    /// <summary>
    /// Issuer of the credential.
    /// </summary>
    [JsonPropertyName("issuer")]
    public string? Issuer { get; set; }
}

/// <summary>
/// Event details in system log.
/// </summary>
public sealed class SystemLogEvent
{
    /// <summary>
    /// Type of event (e.g., access.door.unlock).
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable message describing the event.
    /// </summary>
    [JsonPropertyName("display_message")]
    public string DisplayMessage { get; set; } = string.Empty;

    /// <summary>
    /// Result of the event (GRANTED, BLOCKED, etc.).
    /// </summary>
    [JsonPropertyName("result")]
    public string Result { get; set; } = string.Empty;

    /// <summary>
    /// Reason for the result, if applicable.
    /// </summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    /// <summary>
    /// Timestamp when the event was published (Unix timestamp in milliseconds).
    /// </summary>
    [JsonPropertyName("published")]
    public long Published { get; set; }
}

/// <summary>
/// Target resource affected by the event.
/// </summary>
public sealed class SystemLogTarget
{
    /// <summary>
    /// Target resource identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Type of target (UAH, door, etc.).
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Display name of the target.
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