using System.Text.Json.Serialization;

namespace Unifi.NET.Access.Models.Users;

/// <summary>
/// Request to create a new user.
/// </summary>
public sealed class CreateUserRequest
{
    /// <summary>
    /// First name of the user.
    /// </summary>
    [JsonPropertyName("first_name")]
    public required string FirstName { get; set; }

    /// <summary>
    /// Last name of the user.
    /// </summary>
    [JsonPropertyName("last_name")]
    public required string LastName { get; set; }

    /// <summary>
    /// Email of the user. UniFi Access Requirement: 1.22.16 or later.
    /// </summary>
    [JsonPropertyName("user_email")]
    public string? UserEmail { get; set; }

    /// <summary>
    /// Employee number of the user.
    /// </summary>
    [JsonPropertyName("employee_number")]
    public string? EmployeeNumber { get; set; }

    /// <summary>
    /// User onboarding date as Unix timestamp.
    /// </summary>
    [JsonPropertyName("onboard_time")]
    public long? OnboardTime { get; set; }
}