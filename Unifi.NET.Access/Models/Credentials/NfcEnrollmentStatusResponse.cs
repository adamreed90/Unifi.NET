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
    public string? Token { get; set; }

    /// <summary>
    /// Display ID of the NFC card.
    /// </summary>
    [JsonPropertyName("id")]
    public string? CardId { get; set; }
}