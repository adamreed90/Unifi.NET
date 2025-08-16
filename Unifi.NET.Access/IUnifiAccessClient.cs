using Unifi.NET.Access.Services;

namespace Unifi.NET.Access;

/// <summary>
/// Main client interface for UniFi Access API.
/// </summary>
public interface IUnifiAccessClient
{
    /// <summary>
    /// Gets the user service for managing users and user groups.
    /// </summary>
    IUserService Users { get; }

    /// <summary>
    /// Gets the access policy service for managing access policies, schedules, and holiday groups.
    /// </summary>
    IAccessPolicyService AccessPolicies { get; }

    /// <summary>
    /// Gets the door service for managing doors and door groups.
    /// </summary>
    IDoorService Doors { get; }

    /// <summary>
    /// Gets the credential service for managing NFC cards and PIN codes.
    /// </summary>
    ICredentialService Credentials { get; }

    /// <summary>
    /// Gets the device service for managing devices.
    /// </summary>
    IDeviceService Devices { get; }

    /// <summary>
    /// Gets the user group service for managing user groups.
    /// </summary>
    IUserGroupService UserGroups { get; }
}