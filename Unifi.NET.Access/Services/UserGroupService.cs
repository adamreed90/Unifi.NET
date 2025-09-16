using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using RestSharp;
using Unifi.NET.Access.Configuration;
using Unifi.NET.Access.Exceptions;
using Unifi.NET.Access.Models;
using Unifi.NET.Access.Models.UserGroups;
using Unifi.NET.Access.Models.Users;
using Unifi.NET.Access.Serialization;

namespace Unifi.NET.Access.Services;

/// <summary>
/// Service implementation for managing user groups in UniFi Access.
/// </summary>
public sealed class UserGroupService : BaseService, IUserGroupService
{
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserGroupService"/> class.
    /// </summary>
    public UserGroupService(RestClient client, UnifiAccessConfiguration configuration) 
        : base(client, configuration)
    {
        _jsonOptions = UnifiAccessJsonContext.CreateOptions();
    }

    /// <inheritdoc />
    public async Task<UserGroupResponse> CreateUserGroupAsync(UserGroupRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return await PostAsync<UserGroupResponse>("/api/v1/developer/user_groups", request, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PaginatedResponse<List<UserGroupResponse>>> GetUserGroupsAsync(int? pageNum = null, int? pageSize = null, CancellationToken cancellationToken = default)
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
        var apiRequest = CreateRequest($"/api/v1/developer/user_groups{queryString}", Method.Get);
        var response = await Client.ExecuteAsync(apiRequest, cancellationToken);

        if (!response.IsSuccessful)
        {
            var statusCode = (int)response.StatusCode;
            var errorMessage = response.ErrorMessage ?? response.Content ?? "Unknown error";
            throw new UnifiAccessException($"API request failed: {errorMessage}", "API_ERROR", statusCode);
        }

        if (string.IsNullOrEmpty(response.Content))
        {
            return new PaginatedResponse<List<UserGroupResponse>>
            {
                Items = new List<UserGroupResponse>(),
                Page = pageNum ?? 1,
                PageSize = pageSize ?? 25,
                Total = 0
            };
        }

        // Deserialize using source-generated JSON
        var jsonTypeInfo = (JsonTypeInfo<UnifiApiResponse<List<UserGroupResponse>>>)_jsonOptions.GetTypeInfo(typeof(UnifiApiResponse<List<UserGroupResponse>>));
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

        return new PaginatedResponse<List<UserGroupResponse>>
        {
            Items = apiResponse.Data ?? new List<UserGroupResponse>(),
            Page = apiResponse.Pagination?.PageNum ?? pageNum ?? 1,
            PageSize = apiResponse.Pagination?.PageSize ?? pageSize ?? 25,
            Total = apiResponse.Pagination?.Total ?? 0
        };
    }

    /// <inheritdoc />
    public async Task<UserGroupResponse> GetUserGroupAsync(string groupId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(groupId);
        return await GetAsync<UserGroupResponse>($"/api/v1/developer/user_groups/{groupId}", cancellationToken);
    }

    /// <inheritdoc />
    public async Task<UserGroupResponse> UpdateUserGroupAsync(string groupId, UserGroupRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(groupId);
        ArgumentNullException.ThrowIfNull(request);
        return await PutAsync<UserGroupResponse>($"/api/v1/developer/user_groups/{groupId}", request, cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteUserGroupAsync(string groupId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(groupId);
        await DeleteAsync($"/api/v1/developer/user_groups/{groupId}", cancellationToken);
    }

    /// <inheritdoc />
    public async Task AssignUsersToGroupAsync(string groupId, AssignUsersToGroupRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(groupId);
        ArgumentNullException.ThrowIfNull(request);
        // API expects a plain array of user IDs, not an object
        await PostAsync<object>($"/api/v1/developer/user_groups/{groupId}/users", request.UserIds, cancellationToken);
    }

    /// <inheritdoc />
    public async Task RemoveUsersFromGroupAsync(string groupId, AssignUsersToGroupRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(groupId);
        ArgumentNullException.ThrowIfNull(request);
        
        // Use DELETE with body for removing users - API expects a plain array
        var restRequest = new RestRequest($"/api/v1/developer/user_groups/{groupId}/users", Method.Delete);
        restRequest.AddHeader("Authorization", $"Bearer {Configuration.ApiToken}");
        restRequest.AddHeader("Content-Type", "application/json");
        restRequest.AddJsonBody(request.UserIds);
        
        var response = await Client.ExecuteAsync(restRequest, cancellationToken);
        if (!response.IsSuccessful)
        {
            throw new InvalidOperationException($"Failed to remove users from group: {response.ErrorMessage ?? response.Content}");
        }
    }

    /// <inheritdoc />
    public async Task<PaginatedResponse<List<UserResponse>>> GetUsersInGroupAsync(string groupId, int? pageNum = null, int? pageSize = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(groupId);

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
        var apiRequest = CreateRequest($"/api/v1/developer/user_groups/{groupId}/users{queryString}", Method.Get);
        var response = await Client.ExecuteAsync(apiRequest, cancellationToken);

        if (!response.IsSuccessful)
        {
            var statusCode = (int)response.StatusCode;
            var errorMessage = response.ErrorMessage ?? response.Content ?? "Unknown error";
            throw new UnifiAccessException($"API request failed: {errorMessage}", "API_ERROR", statusCode);
        }

        if (string.IsNullOrEmpty(response.Content))
        {
            return new PaginatedResponse<List<UserResponse>>
            {
                Items = new List<UserResponse>(),
                Page = pageNum ?? 1,
                PageSize = pageSize ?? 25,
                Total = 0
            };
        }

        // Deserialize using source-generated JSON
        var jsonTypeInfo = (JsonTypeInfo<UnifiApiResponse<List<UserResponse>>>)_jsonOptions.GetTypeInfo(typeof(UnifiApiResponse<List<UserResponse>>));
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

        return new PaginatedResponse<List<UserResponse>>
        {
            Items = apiResponse.Data ?? new List<UserResponse>(),
            Page = apiResponse.Pagination?.PageNum ?? pageNum ?? 1,
            PageSize = apiResponse.Pagination?.PageSize ?? pageSize ?? 25,
            Total = apiResponse.Pagination?.Total ?? 0
        };
    }

    /// <inheritdoc />
    public async Task<IEnumerable<UserGroupResponse>> SearchUserGroupsAsync(string keyword, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(keyword);
        var groups = await GetAsync<List<UserGroupResponse>>($"/api/v1/developer/user_groups/search?keyword={Uri.EscapeDataString(keyword)}", cancellationToken);
        return groups ?? new List<UserGroupResponse>();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ImportUserGroupsResponse>> ImportUserGroupsAsync(ImportUserGroupsRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        var restRequest = new RestRequest("/api/v1/developer/user_groups/import", Method.Post);
        restRequest.AddHeader("Authorization", $"Bearer {Configuration.ApiToken}");
        restRequest.AlwaysMultipartFormData = true;
        restRequest.AddFile("file", request.FileContent, request.FileName);
        
        var response = await Client.ExecuteAsync(restRequest, cancellationToken);
        if (!response.IsSuccessful)
        {
            throw new InvalidOperationException($"Failed to import user groups: {response.ErrorMessage ?? response.Content}");
        }

        // Parse the response using source-generated JSON for AOT compatibility
        var jsonTypeInfo = (JsonTypeInfo<UnifiApiResponse<List<ImportUserGroupsResponse>>>)_jsonOptions.GetTypeInfo(typeof(UnifiApiResponse<List<ImportUserGroupsResponse>>));
        var apiResponse = JsonSerializer.Deserialize(response.Content ?? "{}", jsonTypeInfo);
        
        return apiResponse?.Data ?? new List<ImportUserGroupsResponse>();
    }
}