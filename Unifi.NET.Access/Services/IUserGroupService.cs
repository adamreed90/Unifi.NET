using Unifi.NET.Access.Models.UserGroups;
using Unifi.NET.Access.Models.Users;

namespace Unifi.NET.Access.Services;

/// <summary>
/// Service for managing user groups in UniFi Access.
/// </summary>
public interface IUserGroupService
{
    /// <summary>
    /// Creates a new user group.
    /// </summary>
    /// <param name="request">The user group request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created user group.</returns>
    Task<UserGroupResponse> CreateUserGroupAsync(UserGroupRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches all user groups.
    /// </summary>
    /// <param name="pageNum">Page number for pagination.</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of user groups.</returns>
    Task<IEnumerable<UserGroupResponse>> GetUserGroupsAsync(int? pageNum = null, int? pageSize = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches a user group by ID.
    /// </summary>
    /// <param name="groupId">The user group ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user group details.</returns>
    Task<UserGroupResponse> GetUserGroupAsync(string groupId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a user group.
    /// </summary>
    /// <param name="groupId">The user group ID.</param>
    /// <param name="request">The update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated user group.</returns>
    Task<UserGroupResponse> UpdateUserGroupAsync(string groupId, UserGroupRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a user group.
    /// </summary>
    /// <param name="groupId">The user group ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteUserGroupAsync(string groupId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns users to a user group.
    /// </summary>
    /// <param name="groupId">The user group ID.</param>
    /// <param name="request">The assignment request with user IDs.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AssignUsersToGroupAsync(string groupId, AssignUsersToGroupRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes users from a user group.
    /// </summary>
    /// <param name="groupId">The user group ID.</param>
    /// <param name="request">The request with user IDs to remove.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task RemoveUsersFromGroupAsync(string groupId, AssignUsersToGroupRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches users in a specific user group.
    /// </summary>
    /// <param name="groupId">The user group ID.</param>
    /// <param name="pageNum">Page number for pagination.</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of users in the group.</returns>
    Task<IEnumerable<UserResponse>> GetUsersInGroupAsync(string groupId, int? pageNum = null, int? pageSize = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches user groups by name.
    /// </summary>
    /// <param name="keyword">The search keyword.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Matching user groups.</returns>
    Task<IEnumerable<UserGroupResponse>> SearchUserGroupsAsync(string keyword, CancellationToken cancellationToken = default);

    /// <summary>
    /// Imports user groups from CSV.
    /// </summary>
    /// <param name="request">The import request with CSV data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of import results.</returns>
    Task<IEnumerable<ImportUserGroupsResponse>> ImportUserGroupsAsync(ImportUserGroupsRequest request, CancellationToken cancellationToken = default);
}