using System.Text.Json.Serialization;

namespace Unifi.NET.Access.Models.Credentials;

/// <summary>
/// Request to update an NFC card.
/// </summary>
public sealed class UpdateNfcCardRequest
{
    /// <summary>
    /// Alias of the NFC card.
    /// </summary>
    [JsonPropertyName("alias")]
    public string? Alias { get; set; }
}