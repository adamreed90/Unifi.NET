using RestSharp;
using Unifi.NET.Access.Configuration;
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
    /// <summary>
    /// Initializes a new instance of the <see cref="UserGroupService"/> class.
    /// </summary>
    public UserGroupService(RestClient client, UnifiAccessConfiguration configuration) 
        : base(client, configuration)
    {
    }

    /// <inheritdoc />
    public async Task<UserGroupResponse> CreateUserGroupAsync(UserGroupRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return await PostAsync<UserGroupResponse>("/api/v1/developer/user_groups", request, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<UserGroupResponse>> GetUserGroupsAsync(int? pageNum = null, int? pageSize = null, CancellationToken cancellationToken = default)
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
        var groups = await GetAsync<List<UserGroupResponse>>($"/api/v1/developer/user_groups{queryString}", cancellationToken);
        return groups ?? new List<UserGroupResponse>();
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
        await PostAsync<object>($"/api/v1/developer/user_groups/{groupId}/users", request, cancellationToken);
    }

    /// <inheritdoc />
    public async Task RemoveUsersFromGroupAsync(string groupId, AssignUsersToGroupRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(groupId);
        ArgumentNullException.ThrowIfNull(request);
        
        // Use DELETE with body for removing users
        var restRequest = new RestRequest($"/api/v1/developer/user_groups/{groupId}/users", Method.Delete);
        restRequest.AddHeader("Authorization", $"Bearer {Configuration.ApiToken}");
        restRequest.AddHeader("Content-Type", "application/json");
        restRequest.AddJsonBody(request);
        
        var response = await Client.ExecuteAsync(restRequest, cancellationToken);
        if (!response.IsSuccessful)
        {
            throw new InvalidOperationException($"Failed to remove users from group: {response.ErrorMessage ?? response.Content}");
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<UserResponse>> GetUsersInGroupAsync(string groupId, int? pageNum = null, int? pageSize = null, CancellationToken cancellationToken = default)
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
        var users = await GetAsync<List<UserResponse>>($"/api/v1/developer/user_groups/{groupId}/users{queryString}", cancellationToken);
        return users ?? new List<UserResponse>();
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
        var apiResponse = System.Text.Json.JsonSerializer.Deserialize(
            response.Content ?? "{}",
            UnifiAccessJsonContext.Default.UnifiApiResponseListImportUserGroupsResponse);
        
        return apiResponse?.Data ?? new List<ImportUserGroupsResponse>();
    }
}