using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Unifi.NET.Access.Serialization.Contexts;

namespace Unifi.NET.Access.Serialization;

/// <summary>
/// Combined JSON serialization context for UniFi Access API models.
/// This provides Native AOT compatibility by combining all service-specific contexts.
/// </summary>
public static class UnifiAccessJsonContext
{
    private static IJsonTypeInfoResolver? _combined;
    private static readonly object _lock = new();

    /// <summary>
    /// Gets the combined type info resolver for all UniFi Access types.
    /// </summary>
    public static IJsonTypeInfoResolver Combined
    {
        get
        {
            if (_combined == null)
            {
                lock (_lock)
                {
                    _combined ??= JsonTypeInfoResolver.Combine(
                        CoreJsonContext.Default,
                        UserJsonContext.Default,
                        UserGroupJsonContext.Default,
                        CredentialJsonContext.Default,
                        AccessPolicyJsonContext.Default,
                        DoorJsonContext.Default,
                        DeviceJsonContext.Default
                    );
                }
            }
            return _combined;
        }
    }

    /// <summary>
    /// Creates default JSON serializer options configured for UniFi Access API.
    /// </summary>
    public static JsonSerializerOptions CreateOptions()
    {
        return new JsonSerializerOptions
        {
            TypeInfoResolver = Combined,
            PropertyNameCaseInsensitive = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false
        };
    }
}