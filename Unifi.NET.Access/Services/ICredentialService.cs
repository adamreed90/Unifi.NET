using Unifi.NET.Access.Models.Credentials;

namespace Unifi.NET.Access.Services;

/// <summary>
/// Service for managing credentials (NFC cards, PIN codes) in UniFi Access.
/// </summary>
public interface ICredentialService
{
    /// <summary>
    /// Generates a new PIN code.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The generated PIN code.</returns>
    Task<string> GeneratePinCodeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates an NFC card enrollment session.
    /// </summary>
    /// <param name="request">The enrollment session request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The enrollment session response.</returns>
    Task<NfcEnrollmentSessionResponse> CreateNfcEnrollmentSessionAsync(CreateNfcEnrollmentSessionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the status of an NFC card enrollment session.
    /// </summary>
    /// <param name="sessionId">The enrollment session ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The enrollment status.</returns>
    Task<NfcEnrollmentStatusResponse> GetNfcEnrollmentStatusAsync(string sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels an NFC card enrollment session.
    /// </summary>
    /// <param name="sessionId">The enrollment session ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task CancelNfcEnrollmentSessionAsync(string sessionId, CancellationToken cancellationToken = default);
}