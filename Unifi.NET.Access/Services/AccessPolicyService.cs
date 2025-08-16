using RestSharp;
using Unifi.NET.Access.Configuration;
using Unifi.NET.Access.Models.AccessPolicies;

namespace Unifi.NET.Access.Services;

/// <summary>
/// Service implementation for managing access policies in UniFi Access.
/// </summary>
public sealed class AccessPolicyService : BaseService, IAccessPolicyService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AccessPolicyService"/> class.
    /// </summary>
    public AccessPolicyService(RestClient client, UnifiAccessConfiguration configuration) 
        : base(client, configuration)
    {
    }

    /// <inheritdoc />
    public async Task<AccessPolicyResponse> CreateAccessPolicyAsync(CreateAccessPolicyRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return await PostAsync<AccessPolicyResponse>("/api/v1/developer/access_policies", request, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<AccessPolicyResponse> UpdateAccessPolicyAsync(string policyId, UpdateAccessPolicyRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(policyId);
        ArgumentNullException.ThrowIfNull(request);
        
        return await PutAsync<AccessPolicyResponse>($"/api/v1/developer/access_policies/{policyId}", request, cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAccessPolicyAsync(string policyId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(policyId);
        await DeleteAsync($"/api/v1/developer/access_policies/{policyId}", cancellationToken);
    }

    /// <inheritdoc />
    public async Task<AccessPolicyResponse> GetAccessPolicyAsync(string policyId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(policyId);
        return await GetAsync<AccessPolicyResponse>($"/api/v1/developer/access_policies/{policyId}", cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<AccessPolicyResponse>> GetAccessPoliciesAsync(CancellationToken cancellationToken = default)
    {
        var policies = await GetAsync<List<AccessPolicyResponse>>("/api/v1/developer/access_policies", cancellationToken);
        return policies ?? new List<AccessPolicyResponse>();
    }
}