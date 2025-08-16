using System.Text.Json.Serialization;

namespace Unifi.NET.Access.Models.Credentials;

/// <summary>
/// Request to import third-party NFC cards.
/// </summary>
public sealed class ImportNfcCardsRequest
{
    /// <summary>
    /// CSV file content with NFC IDs and optional aliases.
    /// Format: "nfc_id,alias" per line.
    /// </summary>
    public required byte[] FileContent { get; set; }

    /// <summary>
    /// File name for the CSV file.
    /// </summary>
    public string FileName { get; set; } = "nfc_cards.csv";
}