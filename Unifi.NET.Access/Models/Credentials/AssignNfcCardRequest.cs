using System.Text.Json.Serialization;

namespace Unifi.NET.Access.Models.Credentials;

/// <summary>
/// Request to assign an NFC card to a user.
/// </summary>
public sealed class AssignNfcCardRequest
{
    /// <summary>
    /// Token of the NFC card.
    /// </summary>
    [JsonPropertyName("token")]
    public required string Token { get; set; }

    /// <summary>
    /// Determine whether to overwrite an NFC card that has already been assigned.
    /// </summary>
    [JsonPropertyName("force_add")]
    public bool? ForceAdd { get; set; }
}