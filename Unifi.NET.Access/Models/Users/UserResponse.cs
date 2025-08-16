using System.Text.Json.Serialization;

namespace Unifi.NET.Access.Models.Users;

/// <summary>
/// User response model.
/// </summary>
public sealed class UserResponse
{
    /// <summary>
    /// Identity ID of the user.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// First name of the user.
    /// </summary>
    [JsonPropertyName("first_name")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Last name of the user.
    /// </summary>
    [JsonPropertyName("last_name")]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Full name of the user.
    /// </summary>
    [JsonPropertyName("full_name")]
    public string? FullName { get; set; }

    /// <summary>
    /// Preferred name of the user.
    /// </summary>
    [JsonPropertyName("alias")]
    public string? Alias { get; set; }

    /// <summary>
    /// Email of the user.
    /// </summary>
    [JsonPropertyName("user_email")]
    public string? UserEmail { get; set; }

    /// <summary>
    /// The status of the user's email.
    /// </summary>
    [JsonPropertyName("email_status")]
    public string? EmailStatus { get; set; }

    /// <summary>
    /// Contact phone number of the user.
    /// </summary>
    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

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

    /// <summary>
    /// NFC cards associated with the user.
    /// </summary>
    [JsonPropertyName("nfc_cards")]
    public List<NfcCardInfo>? NfcCards { get; set; }

    /// <summary>
    /// License plates associated with the user.
    /// </summary>
    [JsonPropertyName("license_plates")]
    public List<LicensePlateInfo>? LicensePlates { get; set; }

    /// <summary>
    /// PIN code associated with the user.
    /// </summary>
    [JsonPropertyName("pin_code")]
    public PinCodeInfo? PinCode { get; set; }

    /// <summary>
    /// Collection of access policy IDs.
    /// </summary>
    [JsonPropertyName("access_policy_ids")]
    public List<string>? AccessPolicyIds { get; set; }

    /// <summary>
    /// All policies assigned to the user.
    /// </summary>
    [JsonPropertyName("access_policies")]
    public List<AccessPolicyInfo>? AccessPolicies { get; set; }

    /// <summary>
    /// Status of the user (ACTIVE, PENDING, or DEACTIVATED).
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>
    /// Touch Pass assigned to the user.
    /// </summary>
    [JsonPropertyName("touch_pass")]
    public TouchPassInfo? TouchPass { get; set; }
}

/// <summary>
/// NFC card information.
/// </summary>
public sealed class NfcCardInfo
{
    /// <summary>
    /// Display ID of the NFC card.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Unique NFC card token.
    /// </summary>
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Type of NFC card.
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }
}

/// <summary>
/// License plate information.
/// </summary>
public sealed class LicensePlateInfo
{
    /// <summary>
    /// Unique ID of the license plate.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// License plate number.
    /// </summary>
    [JsonPropertyName("credential")]
    public string Credential { get; set; } = string.Empty;

    /// <summary>
    /// Type of credential (should be "license").
    /// </summary>
    [JsonPropertyName("credential_type")]
    public string CredentialType { get; set; } = "license";

    /// <summary>
    /// Status of the credential (active or deactivate).
    /// </summary>
    [JsonPropertyName("credential_status")]
    public string CredentialStatus { get; set; } = "active";
}

/// <summary>
/// PIN code information.
/// </summary>
public sealed class PinCodeInfo
{
    /// <summary>
    /// The user's PIN hash code credential for unlocking a door.
    /// </summary>
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;
}

/// <summary>
/// Access policy information.
/// </summary>
public sealed class AccessPolicyInfo
{
    /// <summary>
    /// Identity ID of the policy.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Name of the policy.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Resources associated with the policy.
    /// </summary>
    [JsonPropertyName("resources")]
    public List<ResourceInfo>? Resources { get; set; }

    /// <summary>
    /// Schedule ID associated with the policy.
    /// </summary>
    [JsonPropertyName("schedule_id")]
    public string? ScheduleId { get; set; }
}

/// <summary>
/// Resource information.
/// </summary>
public sealed class ResourceInfo
{
    /// <summary>
    /// Resource ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Resource type (door or door_group).
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
}

/// <summary>
/// Touch Pass information.
/// </summary>
public sealed class TouchPassInfo
{
    /// <summary>
    /// Touch Pass ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Card ID.
    /// </summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>
    /// Card name.
    /// </summary>
    [JsonPropertyName("card_name")]
    public string? CardName { get; set; }

    /// <summary>
    /// Status of the Touch Pass.
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>
    /// User email associated with Touch Pass.
    /// </summary>
    [JsonPropertyName("user_email")]
    public string? UserEmail { get; set; }

    /// <summary>
    /// User name associated with Touch Pass.
    /// </summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

    /// <summary>
    /// User status.
    /// </summary>
    [JsonPropertyName("user_status")]
    public string? UserStatus { get; set; }

    /// <summary>
    /// Last activity timestamp.
    /// </summary>
    [JsonPropertyName("last_activity")]
    public string? LastActivity { get; set; }
}