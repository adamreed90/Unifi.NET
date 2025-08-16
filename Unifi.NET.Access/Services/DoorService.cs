using RestSharp;
using Unifi.NET.Access.Configuration;
using Unifi.NET.Access.Models.Doors;

namespace Unifi.NET.Access.Services;

/// <summary>
/// Service implementation for managing doors in UniFi Access.
/// </summary>
public sealed class DoorService : BaseService, IDoorService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DoorService"/> class.
    /// </summary>
    public DoorService(RestClient client, UnifiAccessConfiguration configuration) 
        : base(client, configuration)
    {
    }

    /// <inheritdoc />
    public async Task<DoorResponse> GetDoorAsync(string doorId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(doorId);
        return await GetAsync<DoorResponse>($"/api/v1/developer/doors/{doorId}", cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<DoorResponse>> GetDoorsAsync(CancellationToken cancellationToken = default)
    {
        var doors = await GetAsync<List<DoorResponse>>("/api/v1/developer/doors", cancellationToken);
        return doors ?? new List<DoorResponse>();
    }

    /// <inheritdoc />
    public async Task UnlockDoorAsync(string doorId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(doorId);
        await PostAsync<object>($"/api/v1/developer/doors/{doorId}/unlock", null, cancellationToken);
    }

    /// <inheritdoc />
    public async Task SetDoorLockingRuleAsync(string doorId, SetDoorLockingRuleRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(doorId);
        ArgumentNullException.ThrowIfNull(request);
        
        await PutAsync<object>($"/api/v1/developer/doors/{doorId}/lock_rule", request, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<DoorLockingRuleResponse> GetDoorLockingRuleAsync(string doorId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(doorId);
        return await GetAsync<DoorLockingRuleResponse>($"/api/v1/developer/doors/{doorId}/lock_rule", cancellationToken);
    }

    /// <inheritdoc />
    public async Task SetDoorEmergencyStatusAsync(string doorId, SetDoorEmergencyStatusRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(doorId);
        ArgumentNullException.ThrowIfNull(request);
        
        await PutAsync<object>($"/api/v1/developer/doors/{doorId}/emergency", request, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<DoorEmergencyStatusResponse> GetDoorEmergencyStatusAsync(string doorId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(doorId);
        return await GetAsync<DoorEmergencyStatusResponse>($"/api/v1/developer/doors/{doorId}/emergency", cancellationToken);
    }
}