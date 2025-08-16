namespace Unifi.NET.Common.Configuration;

/// <summary>
/// Configuration options for UniFi API clients.
/// </summary>
public class UnifiConfiguration
{
    /// <summary>
    /// Gets or sets the base URL of the UniFi controller.
    /// </summary>
    public required string BaseUrl { get; set; }

    /// <summary>
    /// Gets or sets the API token for authentication.
    /// </summary>
    public required string ApiToken { get; set; }

    /// <summary>
    /// Gets or sets whether to validate SSL certificates.
    /// Default is true for production environments.
    /// </summary>
    public bool ValidateSsl { get; set; } = true;

    /// <summary>
    /// Gets or sets the timeout for HTTP requests in seconds.
    /// Default is 30 seconds.
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Gets or sets the maximum number of retry attempts for failed requests.
    /// Default is 3.
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Gets or sets the base delay in milliseconds between retry attempts.
    /// Default is 1000ms.
    /// </summary>
    public int RetryDelayMilliseconds { get; set; } = 1000;
}