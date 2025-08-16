using Unifi.NET.Access.Models.Doors;

namespace Unifi.NET.Access.Services;

/// <summary>
/// Service for managing doors and door groups in UniFi Access.
/// </summary>
public interface IDoorService
{
    /// <summary>
    /// Fetches a door by ID.
    /// </summary>
    /// <param name="doorId">The door ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The door details.</returns>
    Task<DoorResponse> GetDoorAsync(string doorId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches all doors.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of doors.</returns>
    Task<IEnumerable<DoorResponse>> GetDoorsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Remotely unlocks a door.
    /// </summary>
    /// <param name="doorId">The door ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UnlockDoorAsync(string doorId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets a temporary door locking rule.
    /// </summary>
    /// <param name="doorId">The door ID.</param>
    /// <param name="request">The door locking rule request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SetDoorLockingRuleAsync(string doorId, SetDoorLockingRuleRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches the door locking rule.
    /// </summary>
    /// <param name="doorId">The door ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The door locking rule.</returns>
    Task<DoorLockingRuleResponse> GetDoorLockingRuleAsync(string doorId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the door emergency status.
    /// </summary>
    /// <param name="doorId">The door ID.</param>
    /// <param name="request">The emergency status request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SetDoorEmergencyStatusAsync(string doorId, SetDoorEmergencyStatusRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches the door emergency status.
    /// </summary>
    /// <param name="doorId">The door ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The door emergency status.</returns>
    Task<DoorEmergencyStatusResponse> GetDoorEmergencyStatusAsync(string doorId, CancellationToken cancellationToken = default);
}