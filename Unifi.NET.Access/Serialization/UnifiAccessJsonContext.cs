using System.Text.Json.Serialization;
using Unifi.NET.Access.Models;
using Unifi.NET.Access.Models.Users;
using Unifi.NET.Access.Models.AccessPolicies;
using Unifi.NET.Access.Models.Credentials;
using Unifi.NET.Access.Models.Devices;
using Unifi.NET.Access.Models.Doors;

namespace Unifi.NET.Access.Serialization;

/// <summary>
/// JSON serialization context for UniFi Access API models.
/// This is required for Native AOT compatibility.
/// </summary>
[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(UnifiApiResponse<UserResponse>))]
[JsonSerializable(typeof(UnifiApiResponse<List<UserResponse>>))]
[JsonSerializable(typeof(UnifiApiResponse<AccessPolicyResponse>))]
[JsonSerializable(typeof(UnifiApiResponse<List<AccessPolicyResponse>>))]
[JsonSerializable(typeof(UnifiApiResponse<DoorResponse>))]
[JsonSerializable(typeof(UnifiApiResponse<List<DoorResponse>>))]
[JsonSerializable(typeof(UnifiApiResponse<DoorLockingRuleResponse>))]
[JsonSerializable(typeof(UnifiApiResponse<DoorEmergencyStatusResponse>))]
[JsonSerializable(typeof(UnifiApiResponse<object>))]
[JsonSerializable(typeof(CreateUserRequest))]
[JsonSerializable(typeof(UpdateUserRequest))]
[JsonSerializable(typeof(UserResponse))]
[JsonSerializable(typeof(CreateAccessPolicyRequest))]
[JsonSerializable(typeof(UpdateAccessPolicyRequest))]
[JsonSerializable(typeof(AccessPolicyResponse))]
[JsonSerializable(typeof(DoorResponse))]
[JsonSerializable(typeof(SetDoorLockingRuleRequest))]
[JsonSerializable(typeof(DoorLockingRuleResponse))]
[JsonSerializable(typeof(SetDoorEmergencyStatusRequest))]
[JsonSerializable(typeof(DoorEmergencyStatusResponse))]
[JsonSerializable(typeof(List<AccessPolicyInfo>))]
[JsonSerializable(typeof(Dictionary<string, string>))]
// Credential models
[JsonSerializable(typeof(CreateNfcEnrollmentSessionRequest))]
[JsonSerializable(typeof(NfcEnrollmentSessionResponse))]
[JsonSerializable(typeof(NfcEnrollmentStatusResponse))]
[JsonSerializable(typeof(AssignNfcCardRequest))]
[JsonSerializable(typeof(AssignPinCodeRequest))]
[JsonSerializable(typeof(UnifiApiResponse<NfcEnrollmentSessionResponse>))]
[JsonSerializable(typeof(UnifiApiResponse<NfcEnrollmentStatusResponse>))]
[JsonSerializable(typeof(UnifiApiResponse<string>))]
// Device models
[JsonSerializable(typeof(DeviceResponse))]
[JsonSerializable(typeof(List<DeviceResponse>))]
[JsonSerializable(typeof(List<List<DeviceResponse>>))]
[JsonSerializable(typeof(UnifiApiResponse<List<List<DeviceResponse>>>))]
internal partial class UnifiAccessJsonContext : JsonSerializerContext
{
}