using System.Text.Json.Serialization;

namespace Unifi.NET.Access.Models.Credentials;

/// <summary>
/// NFC card response model.
/// </summary>
public sealed class NfcCardResponse
{
    /// <summary>
    /// Token of the NFC card.
    /// </summary>
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Display ID of the NFC card.
    /// </summary>
    [JsonPropertyName("display_id")]
    public string DisplayId { get; set; } = string.Empty;

    /// <summary>
    /// Status of the NFC card (assigned, pending, disable, deleted, loss).
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Alias of the NFC card.
    /// </summary>
    [JsonPropertyName("alias")]
    public string? Alias { get; set; }

    /// <summary>
    /// Type of the NFC card.
    /// </summary>
    [JsonPropertyName("card_type")]
    public string? CardType { get; set; }

    /// <summary>
    /// Note for the NFC card.
    /// </summary>
    [JsonPropertyName("note")]
    public string? Note { get; set; }

    /// <summary>
    /// Owner ID of the NFC card.
    /// </summary>
    [JsonPropertyName("user_id")]
    public string? UserId { get; set; }

    /// <summary>
    /// Type of the owner (USER or VISITOR).
    /// </summary>
    [JsonPropertyName("user_type")]
    public string? UserType { get; set; }

    /// <summary>
    /// Owner of the NFC card.
    /// </summary>
    [JsonPropertyName("user")]
    public NfcCardUserInfo? User { get; set; }
}

/// <summary>
/// NFC card user information.
/// </summary>
public sealed class NfcCardUserInfo
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
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Avatar URL of the user.
    /// </summary>
    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; }
}