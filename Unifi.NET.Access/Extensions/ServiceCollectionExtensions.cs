using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using Unifi.NET.Access.Configuration;

namespace Unifi.NET.Access.Extensions;

/// <summary>
/// Extension methods for configuring UniFi Access services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds UniFi Access services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">Action to configure UniFi Access options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddUnifiAccess(
        this IServiceCollection services,
        Action<UnifiAccessConfiguration> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureOptions);

        // Configure options
        var configuration = new UnifiAccessConfiguration 
        { 
            BaseUrl = string.Empty, 
            ApiToken = string.Empty 
        };
        configureOptions(configuration);
        configuration.Validate();

        // Register configuration as singleton
        services.AddSingleton(configuration);

        // Register the client as singleton
        services.AddSingleton<IUnifiAccessClient, UnifiAccessClient>();

        // Configure HTTP client with resilience
        services.AddHttpClient<UnifiAccessClient>()
            .AddStandardResilienceHandler(options =>
            {
                options.Retry.MaxRetryAttempts = configuration.MaxRetryAttempts;
                options.Retry.Delay = TimeSpan.FromSeconds(1);
                options.Retry.UseJitter = true;
                options.Retry.BackoffType = DelayBackoffType.Exponential;
                
                options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(configuration.TimeoutSeconds);
                options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(configuration.TimeoutSeconds * 3);
                
                options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(30);
                options.CircuitBreaker.SamplingDuration = TimeSpan.FromMinutes(1);
            });

        return services;
    }

    /// <summary>
    /// Adds UniFi Access services to the service collection using configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="sectionName">The configuration section name. Default is "UnifiAccess".</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddUnifiAccess(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = "UnifiAccess")
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(sectionName);
        if (!section.Exists())
        {
            throw new InvalidOperationException($"Configuration section '{sectionName}' not found.");
        }

        var options = new UnifiAccessConfiguration 
        { 
            BaseUrl = string.Empty, 
            ApiToken = string.Empty 
        };
        section.Bind(options);

        return services.AddUnifiAccess(opt =>
        {
            opt.BaseUrl = options.BaseUrl;
            opt.ApiToken = options.ApiToken;
            opt.TimeoutSeconds = options.TimeoutSeconds;
            opt.ValidateSslCertificate = options.ValidateSslCertificate;
            opt.MaxRetryAttempts = options.MaxRetryAttempts;
        });
    }
}