using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using RestSharp;
using Unifi.NET.Access.Configuration;
using Unifi.NET.Access.Exceptions;
using Unifi.NET.Access.Models;
using Unifi.NET.Access.Models.Credentials;
using Unifi.NET.Access.Serialization;

namespace Unifi.NET.Access.Services;

/// <summary>
/// Service implementation for managing credentials in UniFi Access.
/// </summary>
public sealed class CredentialService : BaseService, ICredentialService
{
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="CredentialService"/> class.
    /// </summary>
    public CredentialService(RestClient client, UnifiAccessConfiguration configuration) 
        : base(client, configuration)
    {
        _jsonOptions = UnifiAccessJsonContext.CreateOptions();
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

    /// <inheritdoc />
    public async Task<PaginatedResponse<List<NfcCardResponse>>> GetNfcCardsAsync(int? pageNum = null, int? pageSize = null, CancellationToken cancellationToken = default)
    {
        var query = new List<string>();
        if (pageNum.HasValue)
        {
            query.Add($"page_num={pageNum.Value}");
        }
        if (pageSize.HasValue)
        {
            query.Add($"page_size={pageSize.Value}");
        }

        var queryString = query.Count > 0 ? "?" + string.Join("&", query) : "";

        // Use direct API call to get pagination info
        var apiRequest = CreateRequest($"/api/v1/developer/credentials/nfc_cards/tokens{queryString}", Method.Get);
        var response = await Client.ExecuteAsync(apiRequest, cancellationToken);

        if (!response.IsSuccessful)
        {
            var statusCode = (int)response.StatusCode;
            var errorMessage = response.ErrorMessage ?? response.Content ?? "Unknown error";
            throw new UnifiAccessException($"API request failed: {errorMessage}", "API_ERROR", statusCode);
        }

        if (string.IsNullOrEmpty(response.Content))
        {
            return new PaginatedResponse<List<NfcCardResponse>>
            {
                Items = new List<NfcCardResponse>(),
                Page = pageNum ?? 1,
                PageSize = pageSize ?? 25,
                Total = 0
            };
        }

        // Deserialize using source-generated JSON
        var jsonTypeInfo = (JsonTypeInfo<UnifiApiResponse<List<NfcCardResponse>>>)_jsonOptions.GetTypeInfo(typeof(UnifiApiResponse<List<NfcCardResponse>>));
        var apiResponse = JsonSerializer.Deserialize(response.Content, jsonTypeInfo);

        if (apiResponse == null)
        {
            throw new UnifiAccessException("Response data is null", "NULL_RESPONSE");
        }

        // Check for API-level errors
        if (apiResponse.Code != "SUCCESS")
        {
            throw UnifiErrorCodeMapper.MapError(apiResponse.Code, apiResponse.Message, (int?)response.StatusCode);
        }

        return new PaginatedResponse<List<NfcCardResponse>>
        {
            Items = apiResponse.Data ?? new List<NfcCardResponse>(),
            Page = apiResponse.Pagination?.PageNum ?? pageNum ?? 1,
            PageSize = apiResponse.Pagination?.PageSize ?? pageSize ?? 25,
            Total = apiResponse.Pagination?.Total ?? 0
        };
    }

    /// <inheritdoc />
    public async Task<NfcCardResponse> GetNfcCardAsync(string token, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(token);
        return await GetAsync<NfcCardResponse>($"/api/v1/developer/credentials/nfc_cards/tokens/{token}", cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteNfcCardAsync(string token, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(token);
        await DeleteAsync($"/api/v1/developer/credentials/nfc_cards/tokens/{token}", cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateNfcCardAsync(string token, UpdateNfcCardRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(token);
        ArgumentNullException.ThrowIfNull(request);
        await PutAsync<object>($"/api/v1/developer/credentials/nfc_cards/tokens/{token}", request, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ImportNfcCardsResponse>> ImportNfcCardsAsync(ImportNfcCardsRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        var restRequest = new RestRequest("/api/v1/developer/credentials/nfc_cards/import", Method.Post);
        restRequest.AddHeader("Authorization", $"Bearer {Configuration.ApiToken}");
        restRequest.AlwaysMultipartFormData = true;
        restRequest.AddFile("file", request.FileContent, request.FileName);
        
        var response = await Client.ExecuteAsync(restRequest, cancellationToken);
        if (!response.IsSuccessful)
        {
            throw new InvalidOperationException($"Failed to import NFC cards: {response.ErrorMessage ?? response.Content}");
        }

        // Parse the response using source-generated JSON for AOT compatibility
        var jsonTypeInfo = (JsonTypeInfo<UnifiApiResponse<List<ImportNfcCardsResponse>>>)_jsonOptions.GetTypeInfo(typeof(UnifiApiResponse<List<ImportNfcCardsResponse>>));
        var apiResponse = JsonSerializer.Deserialize(response.Content ?? "{}", jsonTypeInfo);
        
        return apiResponse?.Data ?? new List<ImportNfcCardsResponse>();
    }
}