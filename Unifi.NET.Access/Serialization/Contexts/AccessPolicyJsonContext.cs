using System.Text.Json.Serialization;
using Unifi.NET.Access.Models;
using Unifi.NET.Access.Models.AccessPolicies;

namespace Unifi.NET.Access.Serialization.Contexts;

/// <summary>
/// JSON serialization context for access policy types.
/// </summary>
[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    GenerationMode = JsonSourceGenerationMode.Metadata)]
// Request/Response types
[JsonSerializable(typeof(CreateAccessPolicyRequest))]
[JsonSerializable(typeof(UpdateAccessPolicyRequest))]
[JsonSerializable(typeof(AccessPolicyResponse))]
[JsonSerializable(typeof(List<AccessPolicyResponse>))]
// API wrapper types
[JsonSerializable(typeof(UnifiApiResponse<AccessPolicyResponse>))]
[JsonSerializable(typeof(UnifiApiResponse<List<AccessPolicyResponse>>))]
internal partial class AccessPolicyJsonContext : JsonSerializerContext
{
}