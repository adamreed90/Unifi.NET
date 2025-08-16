using System.Text.Json.Serialization;

namespace Unifi.NET.Access.Models.Credentials;

/// <summary>
/// Request to assign a PIN code to a user.
/// </summary>
public sealed class AssignPinCodeRequest
{
    /// <summary>
    /// PIN code for the user to unlock doors.
    /// </summary>
    [JsonPropertyName("pin_code")]
    public required string PinCode { get; set; }
}