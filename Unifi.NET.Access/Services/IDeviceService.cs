using Unifi.NET.Access.Models.Devices;

namespace Unifi.NET.Access.Services;

/// <summary>
/// Service for managing devices in UniFi Access.
/// </summary>
public interface IDeviceService
{
    /// <summary>
    /// Fetches all devices.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of devices.</returns>
    Task<IEnumerable<DeviceResponse>> GetDevicesAsync(CancellationToken cancellationToken = default);
}