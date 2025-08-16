using Unifi.NET.Access.Models.AccessPolicies;

namespace Unifi.NET.Access.Services;

/// <summary>
/// Service for managing access policies, schedules, and holiday groups in UniFi Access.
/// </summary>
public interface IAccessPolicyService
{
    /// <summary>
    /// Creates a new access policy.
    /// </summary>
    /// <param name="request">The access policy creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created access policy.</returns>
    Task<AccessPolicyResponse> CreateAccessPolicyAsync(CreateAccessPolicyRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing access policy.
    /// </summary>
    /// <param name="policyId">The access policy ID.</param>
    /// <param name="request">The access policy update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated access policy.</returns>
    Task<AccessPolicyResponse> UpdateAccessPolicyAsync(string policyId, UpdateAccessPolicyRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an access policy.
    /// </summary>
    /// <param name="policyId">The access policy ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteAccessPolicyAsync(string policyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches an access policy by ID.
    /// </summary>
    /// <param name="policyId">The access policy ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The access policy.</returns>
    Task<AccessPolicyResponse> GetAccessPolicyAsync(string policyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches all access policies.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of access policies.</returns>
    Task<IEnumerable<AccessPolicyResponse>> GetAccessPoliciesAsync(CancellationToken cancellationToken = default);
}