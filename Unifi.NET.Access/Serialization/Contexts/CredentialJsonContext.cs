using System.Text.Json.Serialization;
using Unifi.NET.Access.Models;
using Unifi.NET.Access.Models.Credentials;

namespace Unifi.NET.Access.Serialization.Contexts;

/// <summary>
/// JSON serialization context for credential types.
/// </summary>
[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    GenerationMode = JsonSourceGenerationMode.Metadata)]
// NFC Card types
[JsonSerializable(typeof(CreateNfcEnrollmentSessionRequest))]
[JsonSerializable(typeof(NfcEnrollmentSessionResponse))]
[JsonSerializable(typeof(NfcEnrollmentStatusResponse))]
[JsonSerializable(typeof(NfcCardResponse))]
[JsonSerializable(typeof(UpdateNfcCardRequest))]
[JsonSerializable(typeof(ImportNfcCardsRequest))]
[JsonSerializable(typeof(ImportNfcCardsResponse))]
[JsonSerializable(typeof(AssignNfcCardRequest))]
[JsonSerializable(typeof(List<NfcCardResponse>))]
[JsonSerializable(typeof(List<ImportNfcCardsResponse>))]
// PIN Code types
[JsonSerializable(typeof(AssignPinCodeRequest))]
// Paginated response types
[JsonSerializable(typeof(PaginatedResponse<List<NfcCardResponse>>))]
// API wrapper types
[JsonSerializable(typeof(UnifiApiResponse<NfcEnrollmentSessionResponse>))]
[JsonSerializable(typeof(UnifiApiResponse<NfcEnrollmentStatusResponse>))]
[JsonSerializable(typeof(UnifiApiResponse<string>))]
[JsonSerializable(typeof(UnifiApiResponse<NfcCardResponse>))]
[JsonSerializable(typeof(UnifiApiResponse<List<NfcCardResponse>>))]
[JsonSerializable(typeof(UnifiApiResponse<List<ImportNfcCardsResponse>>))]
internal partial class CredentialJsonContext : JsonSerializerContext
{
}