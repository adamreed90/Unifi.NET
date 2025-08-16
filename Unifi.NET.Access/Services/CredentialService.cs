using RestSharp;
using Unifi.NET.Access.Configuration;
using Unifi.NET.Access.Models.Credentials;

namespace Unifi.NET.Access.Services;

/// <summary>
/// Service implementation for managing credentials in UniFi Access.
/// </summary>
public sealed class CredentialService : BaseService, ICredentialService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CredentialService"/> class.
    /// </summary>
    public CredentialService(RestClient client, UnifiAccessConfiguration configuration) 
        : base(client, configuration)
    {
    }

    /// <inheritdoc />
    public async Task<string> GeneratePinCodeAsync(CancellationToken cancellationToken = default)
    {
        // The API returns the PIN code directly in the data field
        var pinCode = await PostAsync<string>("/api/v1/developer/credentials/pin_codes", null, cancellationToken);
        return pinCode ?? throw new InvalidOperationException("Failed to generate PIN code");
    }

    /// <inheritdoc />
    public async Task<NfcEnrollmentSessionResponse> CreateNfcEnrollmentSessionAsync(CreateNfcEnrollmentSessionRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return await PostAsync<NfcEnrollmentSessionResponse>("/api/v1/developer/credentials/nfc_cards/sessions", request, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<NfcEnrollmentStatusResponse> GetNfcEnrollmentStatusAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(sessionId);
        return await GetAsync<NfcEnrollmentStatusResponse>($"/api/v1/developer/credentials/nfc_cards/sessions/{sessionId}", cancellationToken);
    }

    /// <inheritdoc />
    public async Task CancelNfcEnrollmentSessionAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(sessionId);
        await DeleteAsync($"/api/v1/developer/credentials/nfc_cards/sessions/{sessionId}", cancellationToken);
    }
}