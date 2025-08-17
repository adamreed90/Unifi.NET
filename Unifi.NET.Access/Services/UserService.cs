using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using RestSharp;
using Unifi.NET.Access.Configuration;
using Unifi.NET.Access.Models;
using Unifi.NET.Access.Models.AccessPolicies;
using Unifi.NET.Access.Models.Credentials;
using Unifi.NET.Access.Models.Users;
using Unifi.NET.Access.Serialization;

namespace Unifi.NET.Access.Services;

/// <summary>
/// Service implementation for managing users in UniFi Access.
/// </summary>
public sealed class UserService : BaseService, IUserService
{
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserService"/> class.
    /// </summary>
    public UserService(RestClient client, UnifiAccessConfiguration configuration) 
        : base(client, configuration)
    {
        _jsonOptions = UnifiAccessJsonContext.CreateOptions();
    }

    /// <inheritdoc />
    public async Task<UserResponse> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return await PostAsync<UserResponse>("/api/v1/developer/users", request, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<UserResponse> UpdateUserAsync(string userId, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);
        ArgumentNullException.ThrowIfNull(request);
        
        await PutAsync<object>($"/api/v1/developer/users/{userId}", request, cancellationToken);
        // API returns null data on success, fetch the updated user
        return await GetUserAsync(userId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<UserResponse> GetUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);
        return await GetAsync<UserResponse>($"/api/v1/developer/users/{userId}?expand[]=access_policy", cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<UserResponse>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        // First, get the total count with a small page
        var countUrl = "/api/v1/developer/users?page_num=1&page_size=1";
        var countRequest = CreateRequest(countUrl, Method.Get);
        var countResponse = await Client.ExecuteAsync(countRequest, cancellationToken);
        
        if (!countResponse.IsSuccessful || string.IsNullOrEmpty(countResponse.Content))
        {
            return new List<UserResponse>();
        }
        
        // Parse to get total count
        var jsonTypeInfo = (JsonTypeInfo<UnifiApiResponse<List<UserResponse>>>)_jsonOptions.GetTypeInfo(typeof(UnifiApiResponse<List<UserResponse>>));
        var paginatedResponse = JsonSerializer.Deserialize(countResponse.Content, jsonTypeInfo);
        
        if (paginatedResponse?.Pagination == null)
        {
            // Fallback to old method if pagination is not available
            var users = await GetAsync<List<UserResponse>>("/api/v1/developer/users?expand[]=access_policy", cancellationToken);
            return users ?? new List<UserResponse>();
        }
        
        var total = paginatedResponse.Pagination.Total;
        
        // Now fetch all users in one request with the total as page size
        // Note: There might be a max page size limit, so we may need to fetch in chunks
        var pageSize = Math.Min(total, 1000); // Assume max 1000 per page
        var allUsers = new List<UserResponse>();
        var pageNum = 1;
        
        while (allUsers.Count < total)
        {
            var url = $"/api/v1/developer/users?expand[]=access_policy&page_num={pageNum}&page_size={pageSize}";
            var request = CreateRequest(url, Method.Get);
            var response = await Client.ExecuteAsync(request, cancellationToken);
            
            if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
            {
                break;
            }
            
            var pageData = JsonSerializer.Deserialize(response.Content, jsonTypeInfo);
            if (pageData?.Data != null)
            {
                allUsers.AddRange(pageData.Data);
            }
            
            if (pageData?.Data == null || pageData.Data.Count == 0 || allUsers.Count >= total)
            {
                break;
            }
            
            pageNum++;
        }
        
        return allUsers;
    }
    
    /// <inheritdoc />
    public async Task<IEnumerable<UserResponse>> GetUsersAsync(int pageNum, int pageSize, CancellationToken cancellationToken = default)
    {
        var url = $"/api/v1/developer/users?expand[]=access_policy&page_num={pageNum}&page_size={pageSize}";
        var users = await GetAsync<List<UserResponse>>(url, cancellationToken);
        return users ?? new List<UserResponse>();
    }

    /// <inheritdoc />
    public async Task DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);
        await DeleteAsync($"/api/v1/developer/users/{userId}", cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<UserResponse>> SearchUsersAsync(string query, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(query);
        var users = await GetAsync<List<UserResponse>>($"/api/v1/developer/users/search?keyword={Uri.EscapeDataString(query)}", cancellationToken);
        return users ?? new List<UserResponse>();
    }

    /// <inheritdoc />
    public async Task UploadUserPhotoAsync(string userId, byte[] imageData, string fileName, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);
        ArgumentNullException.ThrowIfNull(imageData);
        ArgumentException.ThrowIfNullOrEmpty(fileName);

        var request = new RestRequest($"/api/v1/developer/users/{userId}/photo", Method.Post);
        request.AddHeader("Authorization", $"Bearer {Configuration.ApiToken}");
        request.AlwaysMultipartFormData = true;
        request.AddFile("photo", imageData, fileName);
        
        var response = await Client.ExecuteAsync(request, cancellationToken);
        if (!response.IsSuccessful)
        {
            throw new InvalidOperationException($"Failed to upload user photo: {response.ErrorMessage ?? response.Content}");
        }
    }

    /// <inheritdoc />
    public async Task AssignAccessPolicyToUserAsync(string userId, string policyId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);
        ArgumentException.ThrowIfNullOrEmpty(policyId);

        var body = new { access_policy_ids = new[] { policyId } };
        await PutAsync<object>($"/api/v1/developer/users/{userId}/access_policies", body, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<AccessPolicyResponse>> GetUserAccessPoliciesAsync(string userId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);
        
        var user = await GetUserAsync(userId, cancellationToken);
        if (user.AccessPolicies == null)
        {
            return new List<AccessPolicyResponse>();
        }

        // Convert AccessPolicyInfo to AccessPolicyResponse
        return user.AccessPolicies.Select(p => new AccessPolicyResponse
        {
            Id = p.Id,
            Name = p.Name,
            Resources = p.Resources?.Select(r => new Models.AccessPolicies.AccessPolicyResource 
            { 
                Id = r.Id, 
                Type = r.Type 
            }).ToList(),
            ScheduleId = p.ScheduleId
        });
    }

    /// <inheritdoc />
    public async Task AssignNfcCardToUserAsync(string userId, AssignNfcCardRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);
        ArgumentNullException.ThrowIfNull(request);
        
        await PutAsync<object>($"/api/v1/developer/users/{userId}/nfc_cards", request, cancellationToken);
    }

    /// <inheritdoc />
    public async Task UnassignNfcCardFromUserAsync(string userId, string cardId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);
        ArgumentException.ThrowIfNullOrEmpty(cardId);
        
        await PutAsync<object>($"/api/v1/developer/users/{userId}/nfc_cards/delete", new { card_id = cardId }, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AssignPinCodeToUserAsync(string userId, AssignPinCodeRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);
        ArgumentNullException.ThrowIfNull(request);
        
        await PutAsync<object>($"/api/v1/developer/users/{userId}/pin_codes", request, cancellationToken);
    }

    /// <inheritdoc />
    public async Task UnassignPinCodeFromUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);
        
        await PutAsync<object>($"/api/v1/developer/users/{userId}/pin_codes", new { pin_code = "" }, cancellationToken);
    }
}