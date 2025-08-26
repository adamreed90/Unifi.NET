using System.Net;
using System.Text.Json;
using RestSharp;
using RestSharp.Serializers.Json;
using Unifi.NET.Access.Configuration;
using Unifi.NET.Access.Serialization;
using Unifi.NET.Access.Services;

namespace Unifi.NET.Access;

/// <summary>
/// Main client for UniFi Access API.
/// </summary>
public sealed class UnifiAccessClient : IUnifiAccessClient, IDisposable
{
    private readonly RestClient _restClient;
    private readonly UserService _userService;
    private readonly AccessPolicyService _accessPolicyService;
    private readonly DoorService _doorService;
    private readonly CredentialService _credentialService;
    private readonly DeviceService _deviceService;
    private readonly UserGroupService _userGroupService;
    private readonly SystemLogService _systemLogService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnifiAccessClient"/> class.
    /// </summary>
    public UnifiAccessClient(UnifiAccessConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        configuration.Validate();

        // Configure RestClient
        var options = new RestClientOptions(configuration.BaseUrl)
        {
            ThrowOnAnyError = false,
            ThrowOnDeserializationError = false,
            Timeout = TimeSpan.FromSeconds(configuration.TimeoutSeconds),
            RemoteCertificateValidationCallback = configuration.ValidateSslCertificate ? null : (sender, cert, chain, errors) => true
        };

        // Configure with source-generated JSON for Native AOT
        var jsonOptions = UnifiAccessJsonContext.CreateOptions();

        _restClient = new RestClient(
            options,
            configureSerialization: s => s.UseSystemTextJson(jsonOptions)
        );

        // Initialize services
        _userService = new UserService(_restClient, configuration);
        _accessPolicyService = new AccessPolicyService(_restClient, configuration);
        _doorService = new DoorService(_restClient, configuration);
        _credentialService = new CredentialService(_restClient, configuration);
        _deviceService = new DeviceService(_restClient, configuration);
        _userGroupService = new UserGroupService(_restClient, configuration);
        _systemLogService = new SystemLogService(_restClient, configuration);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnifiAccessClient"/> class with a custom RestClient.
    /// This constructor is primarily for testing purposes.
    /// </summary>
    internal UnifiAccessClient(UnifiAccessConfiguration configuration, RestClient restClient)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(restClient);
        configuration.Validate();

        _restClient = restClient;

        // Initialize services
        _userService = new UserService(_restClient, configuration);
        _accessPolicyService = new AccessPolicyService(_restClient, configuration);
        _doorService = new DoorService(_restClient, configuration);
        _credentialService = new CredentialService(_restClient, configuration);
        _deviceService = new DeviceService(_restClient, configuration);
        _userGroupService = new UserGroupService(_restClient, configuration);
        _systemLogService = new SystemLogService(_restClient, configuration);
    }

    /// <inheritdoc />
    public IUserService Users => _userService;

    /// <inheritdoc />
    public IAccessPolicyService AccessPolicies => _accessPolicyService;

    /// <inheritdoc />
    public IDoorService Doors => _doorService;

    /// <inheritdoc />
    public ICredentialService Credentials => _credentialService;

    /// <inheritdoc />
    public IDeviceService Devices => _deviceService;

    /// <inheritdoc />
    public IUserGroupService UserGroups => _userGroupService;

    /// <inheritdoc />
    public ISystemLogService SystemLogs => _systemLogService;

    /// <summary>
    /// Disposes the client and its resources.
    /// </summary>
    public void Dispose()
    {
        _restClient?.Dispose();
    }
}
