using System.Text.Json.Serialization;

namespace Unifi.NET.Access.Models.Credentials;

/// <summary>
/// Response from creating an NFC card enrollment session.
/// </summary>
public sealed class NfcEnrollmentSessionResponse
{
    /// <summary>
    /// The session ID for enrolling an NFC card.
    /// </summary>
    [JsonPropertyName("session_id")]
    public string SessionId { get; set; } = string.Empty;
}