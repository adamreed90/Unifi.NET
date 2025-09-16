using Unifi.NET.Access.Models.Credentials;
using Unifi.NET.Access.Models;

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

    /// <summary>
    /// Fetches all NFC cards.
    /// </summary>
    /// <param name="pageNum">Page number for pagination.</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of NFC cards.</returns>
    Task<PaginatedResponse<List<NfcCardResponse>>> GetNfcCardsAsync(int? pageNum = null, int? pageSize = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches NFC card details by token.
    /// </summary>
    /// <param name="token">The NFC card token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The NFC card details.</returns>
    Task<NfcCardResponse> GetNfcCardAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an NFC card.
    /// </summary>
    /// <param name="token">The NFC card token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteNfcCardAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an NFC card.
    /// </summary>
    /// <param name="token">The NFC card token.</param>
    /// <param name="request">The update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UpdateNfcCardAsync(string token, UpdateNfcCardRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Imports third-party NFC cards from CSV.
    /// </summary>
    /// <param name="request">The import request with CSV data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of import results.</returns>
    Task<IEnumerable<ImportNfcCardsResponse>> ImportNfcCardsAsync(ImportNfcCardsRequest request, CancellationToken cancellationToken = default);
}