using System.Text.Json.Serialization;

namespace Unifi.NET.Access.Models.Credentials;

/// <summary>
/// Response from importing NFC cards.
/// </summary>
public sealed class ImportNfcCardsResponse
{
    /// <summary>
    /// NFC ID from the import.
    /// </summary>
    [JsonPropertyName("nfc_id")]
    public string NfcId { get; set; } = string.Empty;

    /// <summary>
    /// Alias for the NFC card.
    /// </summary>
    [JsonPropertyName("alias")]
    public string? Alias { get; set; }

    /// <summary>
    /// Generated token for the NFC card. Empty if import failed.
    /// </summary>
    [JsonPropertyName("token")]
    public string? Token { get; set; }
}