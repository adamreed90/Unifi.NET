using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using RestSharp;
using Unifi.NET.Access.Configuration;
using Unifi.NET.Access.Exceptions;
using Unifi.NET.Access.Models;
using Unifi.NET.Access.Serialization;

namespace Unifi.NET.Access.Services;

/// <summary>
/// Base service class for UniFi Access API operations.
/// </summary>
public abstract class BaseService
{
    /// <summary>
    /// Gets the REST client.
    /// </summary>
    protected RestClient Client { get; }
    
    /// <summary>
    /// Gets the configuration.
    /// </summary>
    protected UnifiAccessConfiguration Configuration { get; }

    /// <summary>
    /// JSON serializer options configured for Native AOT.
    /// </summary>
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseService"/> class.
    /// </summary>
    protected BaseService(RestClient client, UnifiAccessConfiguration configuration)
    {
        Client = client;
        Configuration = configuration;
        _jsonOptions = UnifiAccessJsonContext.CreateOptions();
    }

    /// <summary>
    /// Executes a GET request.
    /// </summary>
    protected async Task<T> GetAsync<T>(string resource, CancellationToken cancellationToken = default)
    {
        var request = CreateRequest(resource, Method.Get);
        return await ExecuteAsync<T>(request, cancellationToken);
    }

    /// <summary>
    /// Executes a POST request.
    /// </summary>
    protected async Task<T> PostAsync<T>(string resource, object? body, CancellationToken cancellationToken = default)
    {
        var request = CreateRequest(resource, Method.Post);
        if (body != null)
        {
            request.AddJsonBody(body);
        }
        return await ExecuteAsync<T>(request, cancellationToken);
    }

    /// <summary>
    /// Executes a PUT request.
    /// </summary>
    protected async Task<T> PutAsync<T>(string resource, object body, CancellationToken cancellationToken = default)
    {
        var request = CreateRequest(resource, Method.Put);
        request.AddJsonBody(body);
        return await ExecuteAsync<T>(request, cancellationToken);
    }

    /// <summary>
    /// Executes a DELETE request.
    /// </summary>
    protected async Task DeleteAsync(string resource, CancellationToken cancellationToken = default)
    {
        var request = CreateRequest(resource, Method.Delete);
        await ExecuteAsync<object>(request, cancellationToken);
    }

    /// <summary>
    /// Creates a REST request with common headers.
    /// </summary>
    private RestRequest CreateRequest(string resource, Method method)
    {
        var request = new RestRequest(resource, method);
        request.AddHeader("Authorization", $"Bearer {Configuration.ApiToken}");
        request.AddHeader("Content-Type", "application/json");
        return request;
    }

    /// <summary>
    /// Executes a request and handles the response.
    /// </summary>
    private async Task<T> ExecuteAsync<T>(RestRequest request, CancellationToken cancellationToken)
    {
        var response = await Client.ExecuteAsync(request, cancellationToken);
        
        if (!response.IsSuccessful)
        {
            HandleErrorResponse(response);
        }

        if (string.IsNullOrEmpty(response.Content))
        {
            return default!;
        }

        // Get the JsonTypeInfo for the specific type from our options
        var jsonTypeInfo = (JsonTypeInfo<UnifiApiResponse<T>>)_jsonOptions.GetTypeInfo(typeof(UnifiApiResponse<T>));
        
        // Deserialize using the type info for Native AOT compatibility
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

        return apiResponse.Data!;
    }

    /// <summary>
    /// Handles error responses from the API.
    /// </summary>
    private void HandleErrorResponse(RestResponse response)
    {
        var statusCode = (int)response.StatusCode;
        var errorMessage = response.ErrorMessage ?? response.Content ?? "Unknown error";

        // Try to parse error response using JsonDocument for AOT compatibility
        if (!string.IsNullOrEmpty(response.Content))
        {
            try
            {
                using var doc = JsonDocument.Parse(response.Content);
                if (doc.RootElement.TryGetProperty("code", out var codeElement) &&
                    doc.RootElement.TryGetProperty("msg", out var msgElement))
                {
                    var code = codeElement.GetString() ?? "UNKNOWN_ERROR";
                    var message = msgElement.GetString() ?? errorMessage;
                    throw UnifiErrorCodeMapper.MapError(code, message, statusCode);
                }
            }
            catch (JsonException)
            {
                // If we can't parse the error, fall through to generic error handling
            }
        }

        // Handle HTTP status codes
        throw statusCode switch
        {
            401 => new UnifiAuthenticationException(errorMessage, "CODE_UNAUTHORIZED", statusCode),
            403 => new UnifiForbiddenException(errorMessage, "CODE_OPERATION_FORBIDDEN", statusCode),
            404 => new UnifiNotFoundException(errorMessage, "CODE_NOT_EXISTS", statusCode),
            429 => new UnifiRateLimitException(errorMessage, "CODE_RATE_LIMIT", statusCode),
            _ => new UnifiAccessException(errorMessage, "CODE_SYSTEM_ERROR", statusCode)
        };
    }
}