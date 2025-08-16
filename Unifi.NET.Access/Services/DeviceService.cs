using RestSharp;
using Unifi.NET.Access.Configuration;
using Unifi.NET.Access.Models.Devices;

namespace Unifi.NET.Access.Services;

/// <summary>
/// Service implementation for managing devices in UniFi Access.
/// </summary>
public sealed class DeviceService : BaseService, IDeviceService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeviceService"/> class.
    /// </summary>
    public DeviceService(RestClient client, UnifiAccessConfiguration configuration) 
        : base(client, configuration)
    {
    }

    /// <inheritdoc />
    public async Task<IEnumerable<DeviceResponse>> GetDevicesAsync(CancellationToken cancellationToken = default)
    {
        // The API returns a nested array structure
        var devices = await GetAsync<List<List<DeviceResponse>>>("/api/v1/developer/devices", cancellationToken);
        
        // Flatten the nested array structure if needed
        if (devices != null && devices.Count > 0)
        {
            return devices[0] ?? new List<DeviceResponse>();
        }
        
        return new List<DeviceResponse>();
    }
}