using System.Text.Json.Serialization;

namespace Unifi.NET.Access.Models.Credentials;

/// <summary>
/// Request to create an NFC card enrollment session.
/// </summary>
public sealed class CreateNfcEnrollmentSessionRequest
{
    /// <summary>
    /// Identity ID of the device (UA reader).
    /// </summary>
    [JsonPropertyName("device_id")]
    public required string DeviceId { get; set; }

    /// <summary>
    /// Option to reset an NFC card already enrolled at another site.
    /// </summary>
    [JsonPropertyName("reset_ua_card")]
    public bool? ResetUaCard { get; set; }
}