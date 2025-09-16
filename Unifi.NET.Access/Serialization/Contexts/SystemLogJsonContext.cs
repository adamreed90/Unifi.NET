using System.Text.Json.Serialization;
using Unifi.NET.Access.Models;
using Unifi.NET.Access.Models.SystemLogs;

namespace Unifi.NET.Access.Serialization.Contexts;

/// <summary>
/// JSON serialization context for system log models.
/// </summary>
[JsonSourceGenerationOptions(WriteIndented = false)]
[JsonSerializable(typeof(SystemLogRequest))]
[JsonSerializable(typeof(SystemLogResponse))]
[JsonSerializable(typeof(SystemLogQueryResponse))]
[JsonSerializable(typeof(SystemLogEntry))]
[JsonSerializable(typeof(SystemLogSource))]
[JsonSerializable(typeof(SystemLogActor))]
[JsonSerializable(typeof(SystemLogAuthentication))]
[JsonSerializable(typeof(SystemLogEvent))]
[JsonSerializable(typeof(SystemLogTarget))]
[JsonSerializable(typeof(SystemLogExportRequest))]
[JsonSerializable(typeof(SystemLogResourceResponse))]
[JsonSerializable(typeof(SystemLogResource))]
[JsonSerializable(typeof(List<SystemLogEntry>))]
[JsonSerializable(typeof(List<SystemLogTarget>))]
[JsonSerializable(typeof(List<SystemLogResource>))]
[JsonSerializable(typeof(UnifiApiResponse<SystemLogResponse>))]
[JsonSerializable(typeof(UnifiApiResponse<SystemLogResourceResponse>))]
internal partial class SystemLogJsonContext : JsonSerializerContext
{
}