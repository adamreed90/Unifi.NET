using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using RestSharp;
using Unifi.NET.Access.Configuration;
using Unifi.NET.Access.Exceptions;
using Unifi.NET.Access.Models;
using Unifi.NET.Access.Models.AccessPolicies;
using Unifi.NET.Access.Models.Doors;
using Unifi.NET.Access.Models.Users;
using Unifi.NET.Access.Serialization;

namespace Unifi.NET.Access.Services;

/// <summary>
/// Base service class with common HTTP operations for UniFi Access API.
/// </summary>
public abstract class BaseService
{
    protected readonly RestClient Client;
    protected readonly UnifiAccessConfiguration Configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseService"/> class.
    /// </summary>
    protected BaseService(RestClient client, UnifiAccessConfiguration configuration)
    {
        Client = client ?? throw new ArgumentNullException(nameof(client));
        Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
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
    protected async Task<T> PostAsync<T>(string resource, object? body = null, CancellationToken cancellationToken = default)
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
    protected async Task<T> PutAsync<T>(string resource, object? body = null, CancellationToken cancellationToken = default)
    {
        var request = CreateRequest(resource, Method.Put);
        if (body != null)
        {
            request.AddJsonBody(body);
        }
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
    /// Creates a RestRequest with common headers.
    /// </summary>
    private RestRequest CreateRequest(string resource, Method method)
    {
        var request = new RestRequest(resource, method);
        request.AddHeader("Authorization", $"Bearer {Configuration.ApiToken}");
        request.AddHeader("Accept", "application/json");
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

        // Use source-generated deserialization for Native AOT
        var typeInfo = GetTypeInfo<UnifiApiResponse<T>>();
        var apiResponse = JsonSerializer.Deserialize(response.Content, typeInfo) as UnifiApiResponse<T>;
        
        if (apiResponse == null)
        {
            throw new UnifiAccessException("Failed to deserialize response", "DESERIALIZATION_ERROR");
        }

        // Check for API-level errors
        if (apiResponse.Code != "SUCCESS")
        {
            throw UnifiErrorCodeMapper.MapError(apiResponse.Code, apiResponse.Message, (int?)response.StatusCode);
        }

        return apiResponse.Data!;
    }

    /// <summary>
    /// Gets the JsonTypeInfo for the specified type.
    /// </summary>
    private static JsonTypeInfo<T> GetTypeInfo<T>()
    {
        var type = typeof(T);
        
        if (type == typeof(UnifiApiResponse<UserResponse>))
        {
            return (JsonTypeInfo<T>)(object)UnifiAccessJsonContext.Default.UnifiApiResponseUserResponse;
        }
        if (type == typeof(UnifiApiResponse<List<UserResponse>>))
        {
            return (JsonTypeInfo<T>)(object)UnifiAccessJsonContext.Default.UnifiApiResponseListUserResponse;
        }
        if (type == typeof(UnifiApiResponse<AccessPolicyResponse>))
        {
            return (JsonTypeInfo<T>)(object)UnifiAccessJsonContext.Default.UnifiApiResponseAccessPolicyResponse;
        }
        if (type == typeof(UnifiApiResponse<List<AccessPolicyResponse>>))
        {
            return (JsonTypeInfo<T>)(object)UnifiAccessJsonContext.Default.UnifiApiResponseListAccessPolicyResponse;
        }
        if (type == typeof(UnifiApiResponse<DoorResponse>))
        {
            return (JsonTypeInfo<T>)(object)UnifiAccessJsonContext.Default.UnifiApiResponseDoorResponse;
        }
        if (type == typeof(UnifiApiResponse<List<DoorResponse>>))
        {
            return (JsonTypeInfo<T>)(object)UnifiAccessJsonContext.Default.UnifiApiResponseListDoorResponse;
        }
        if (type == typeof(UnifiApiResponse<DoorLockingRuleResponse>))
        {
            return (JsonTypeInfo<T>)(object)UnifiAccessJsonContext.Default.UnifiApiResponseDoorLockingRuleResponse;
        }
        if (type == typeof(UnifiApiResponse<DoorEmergencyStatusResponse>))
        {
            return (JsonTypeInfo<T>)(object)UnifiAccessJsonContext.Default.UnifiApiResponseDoorEmergencyStatusResponse;
        }
        if (type == typeof(UnifiApiResponse<object>))
        {
            return (JsonTypeInfo<T>)(object)UnifiAccessJsonContext.Default.UnifiApiResponseObject;
        }
        
        throw new NotSupportedException($"Type {type} is not registered for Native AOT deserialization");
    }

    /// <summary>
    /// Handles error responses from the API.
    /// </summary>
    private void HandleErrorResponse(RestResponse response)
    {
        var statusCode = (int)response.StatusCode;
        var errorMessage = response.ErrorMessage ?? response.Content ?? "Unknown error";

        // Try to parse error response
        if (!string.IsNullOrEmpty(response.Content))
        {
            try
            {
                var errorResponse = JsonSerializer.Deserialize(response.Content, UnifiAccessJsonContext.Default.UnifiApiResponseObject) as UnifiApiResponse<object>;
                if (errorResponse != null)
                {
                    throw UnifiErrorCodeMapper.MapError(errorResponse.Code, errorResponse.Message, statusCode);
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