using System.Text.Json.Serialization;

namespace Unifi.NET.Access.Models.Credentials;

/// <summary>
/// Response from polling NFC card enrollment status.
/// </summary>
public sealed class NfcEnrollmentStatusResponse
{
    /// <summary>
    /// Unique NFC card token.
    /// </summary>
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Display ID of the NFC card.
    /// </summary>
    [JsonPropertyName("card_id")]
    public string CardId { get; set; } = string.Empty;
}