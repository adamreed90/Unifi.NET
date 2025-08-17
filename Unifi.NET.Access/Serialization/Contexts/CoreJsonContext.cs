using System.Text.Json.Serialization;
using Unifi.NET.Access.Models;

namespace Unifi.NET.Access.Serialization.Contexts;

/// <summary>
/// JSON serialization context for core UniFi Access types.
/// </summary>
[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    GenerationMode = JsonSourceGenerationMode.Metadata)]
[JsonSerializable(typeof(UnifiApiResponse<object>))]
[JsonSerializable(typeof(UnifiApiResponse<string>))]
[JsonSerializable(typeof(Dictionary<string, string>))]
internal partial class CoreJsonContext : JsonSerializerContext
{
}