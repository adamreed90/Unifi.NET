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
    private readonly UnifiAccessConfiguration _configuration;
    private readonly UserService _userService;
    private readonly AccessPolicyService _accessPolicyService;
    private readonly DoorService _doorService;
    private readonly CredentialService _credentialService;
    private readonly DeviceService _deviceService;
    private readonly UserGroupService _userGroupService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnifiAccessClient"/> class.
    /// </summary>
    public UnifiAccessClient(UnifiAccessConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        configuration.Validate();

        _configuration = configuration;

        // Configure RestClient
        var options = new RestClientOptions(_configuration.BaseUrl)
        {
            ThrowOnAnyError = false,
            ThrowOnDeserializationError = false,
            Timeout = TimeSpan.FromSeconds(_configuration.TimeoutSeconds),
            RemoteCertificateValidationCallback = _configuration.ValidateSslCertificate ? null : (sender, cert, chain, errors) => true
        };

        // Configure with source-generated JSON for Native AOT
        var jsonOptions = UnifiAccessJsonContext.CreateOptions();

        _restClient = new RestClient(
            options,
            configureSerialization: s => s.UseSystemTextJson(jsonOptions)
        );

        // Initialize services
        _userService = new UserService(_restClient, _configuration);
        _accessPolicyService = new AccessPolicyService(_restClient, _configuration);
        _doorService = new DoorService(_restClient, _configuration);
        _credentialService = new CredentialService(_restClient, _configuration);
        _deviceService = new DeviceService(_restClient, _configuration);
        _userGroupService = new UserGroupService(_restClient, _configuration);
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

        _configuration = configuration;
        _restClient = restClient;

        // Initialize services
        _userService = new UserService(_restClient, _configuration);
        _accessPolicyService = new AccessPolicyService(_restClient, _configuration);
        _doorService = new DoorService(_restClient, _configuration);
        _credentialService = new CredentialService(_restClient, _configuration);
        _deviceService = new DeviceService(_restClient, _configuration);
        _userGroupService = new UserGroupService(_restClient, _configuration);
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

    /// <summary>
    /// Disposes the client and its resources.
    /// </summary>
    public void Dispose()
    {
        _restClient?.Dispose();
    }
}