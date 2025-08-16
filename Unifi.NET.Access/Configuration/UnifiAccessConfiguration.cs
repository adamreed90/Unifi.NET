namespace Unifi.NET.Access.Configuration;

/// <summary>
/// Configuration for UniFi Access API client.
/// </summary>
public sealed class UnifiAccessConfiguration
{
    /// <summary>
    /// The base URL for the UniFi Access API (e.g., https://console-ip:12445).
    /// </summary>
    public required string BaseUrl { get; set; }

    /// <summary>
    /// The API token for authentication.
    /// </summary>
    public required string ApiToken { get; set; }

    /// <summary>
    /// Timeout for HTTP requests in seconds. Default is 30 seconds.
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Whether to validate SSL certificates. Default is true.
    /// Set to false for self-signed certificates (not recommended for production).
    /// </summary>
    public bool ValidateSslCertificate { get; set; } = true;

    /// <summary>
    /// Maximum number of retry attempts for failed requests. Default is 3.
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Validates the configuration.
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(BaseUrl))
        {
            throw new ArgumentException("BaseUrl is required", nameof(BaseUrl));
        }

        if (string.IsNullOrWhiteSpace(ApiToken))
        {
            throw new ArgumentException("ApiToken is required", nameof(ApiToken));
        }

        if (!Uri.TryCreate(BaseUrl, UriKind.Absolute, out var uri) || 
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException("BaseUrl must be a valid HTTP or HTTPS URL", nameof(BaseUrl));
        }

        if (TimeoutSeconds <= 0)
        {
            throw new ArgumentException("TimeoutSeconds must be greater than 0", nameof(TimeoutSeconds));
        }

        if (MaxRetryAttempts < 0)
        {
            throw new ArgumentException("MaxRetryAttempts must be 0 or greater", nameof(MaxRetryAttempts));
        }
    }
}