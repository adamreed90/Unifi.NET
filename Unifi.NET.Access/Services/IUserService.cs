using Unifi.NET.Access.Models.AccessPolicies;
using Unifi.NET.Access.Models.Users;

namespace Unifi.NET.Access.Services;

/// <summary>
/// Service for managing users and user groups in UniFi Access.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Creates a new user registration.
    /// </summary>
    /// <param name="request">The user registration request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created user.</returns>
    Task<UserResponse> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="request">The user update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated user.</returns>
    Task<UserResponse> UpdateUserAsync(string userId, UpdateUserRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches a user by ID.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user.</returns>
    Task<UserResponse> GetUserAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches all users.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of users.</returns>
    Task<IEnumerable<UserResponse>> GetUsersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteUserAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for users.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of matching users.</returns>
    Task<IEnumerable<UserResponse>> SearchUsersAsync(string query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads a user profile picture.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="imageData">The image data.</param>
    /// <param name="fileName">The file name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UploadUserPhotoAsync(string userId, byte[] imageData, string fileName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns an access policy to a user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="policyId">The access policy ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AssignAccessPolicyToUserAsync(string userId, string policyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches access policies assigned to a user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of access policies.</returns>
    Task<IEnumerable<AccessPolicyResponse>> GetUserAccessPoliciesAsync(string userId, CancellationToken cancellationToken = default);
}