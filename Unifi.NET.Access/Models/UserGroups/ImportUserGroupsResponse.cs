using System.Text.Json.Serialization;

namespace Unifi.NET.Access.Models.UserGroups;

/// <summary>
/// Response from importing user groups.
/// </summary>
public sealed class ImportUserGroupsResponse
{
    /// <summary>
    /// Name of the imported user group.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Generated ID for the user group. Empty if import failed.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Description of the user group.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Parent group ID if specified.
    /// </summary>
    [JsonPropertyName("parent_id")]
    public string? ParentId { get; set; }

    /// <summary>
    /// Whether the import was successful.
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>
    /// Error message if import failed.
    /// </summary>
    [JsonPropertyName("error")]
    public string? Error { get; set; }
}