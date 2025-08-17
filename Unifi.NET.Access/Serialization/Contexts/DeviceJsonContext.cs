using System.Text.Json.Serialization;
using Unifi.NET.Access.Models;
using Unifi.NET.Access.Models.Devices;

namespace Unifi.NET.Access.Serialization.Contexts;

/// <summary>
/// JSON serialization context for device types.
/// </summary>
[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    GenerationMode = JsonSourceGenerationMode.Metadata)]
// Device types
[JsonSerializable(typeof(DeviceResponse))]
[JsonSerializable(typeof(List<DeviceResponse>))]
[JsonSerializable(typeof(List<List<DeviceResponse>>))]
// API wrapper types
[JsonSerializable(typeof(UnifiApiResponse<List<List<DeviceResponse>>>))]
internal partial class DeviceJsonContext : JsonSerializerContext
{
}