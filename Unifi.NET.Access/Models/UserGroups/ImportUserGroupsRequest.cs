using System.Text.Json.Serialization;

namespace Unifi.NET.Access.Models.UserGroups;

/// <summary>
/// Request to import user groups from CSV.
/// </summary>
public sealed class ImportUserGroupsRequest
{
    /// <summary>
    /// CSV file content with user group data.
    /// Format: "name,description,parent_id" per line.
    /// </summary>
    public required byte[] FileContent { get; set; }

    /// <summary>
    /// File name for the CSV file.
    /// </summary>
    public string FileName { get; set; } = "user_groups.csv";
}