using Unifi.NET.Common.Configuration;

namespace Unifi.NET.Common.Abstractions;

/// <summary>
/// Base interface for all UniFi API clients.
/// </summary>
public interface IUnifiClient
{
    /// <summary>
    /// Gets the configuration for this client.
    /// </summary>
    UnifiConfiguration Configuration { get; }
}