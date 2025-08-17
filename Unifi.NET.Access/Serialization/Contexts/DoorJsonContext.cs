using System.Text.Json.Serialization;
using Unifi.NET.Access.Models;
using Unifi.NET.Access.Models.Doors;

namespace Unifi.NET.Access.Serialization.Contexts;

/// <summary>
/// JSON serialization context for door-related types.
/// </summary>
[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    GenerationMode = JsonSourceGenerationMode.Metadata)]
// Request/Response types
[JsonSerializable(typeof(DoorResponse))]
[JsonSerializable(typeof(SetDoorLockingRuleRequest))]
[JsonSerializable(typeof(DoorLockingRuleResponse))]
[JsonSerializable(typeof(SetDoorEmergencyStatusRequest))]
[JsonSerializable(typeof(DoorEmergencyStatusResponse))]
[JsonSerializable(typeof(List<DoorResponse>))]
// API wrapper types
[JsonSerializable(typeof(UnifiApiResponse<DoorResponse>))]
[JsonSerializable(typeof(UnifiApiResponse<List<DoorResponse>>))]
[JsonSerializable(typeof(UnifiApiResponse<DoorLockingRuleResponse>))]
[JsonSerializable(typeof(UnifiApiResponse<DoorEmergencyStatusResponse>))]
internal partial class DoorJsonContext : JsonSerializerContext
{
}