# Unifi.NET

A comprehensive collection of .NET SDKs for interacting with Ubiquiti UniFi APIs and services. This solution provides strongly-typed, async-first client libraries for UniFi's ecosystem, with built-in resilience and dependency injection support.

[![NuGet](https://img.shields.io/nuget/v/Unifi.NET.Access.svg)](https://www.nuget.org/packages/Unifi.NET.Access/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4)](https://dotnet.microsoft.com/download)

## 📦 Available Packages

| Package | Description | Status | NuGet |
|---------|-------------|--------|-------|
| **Unifi.NET.Access** | SDK for UniFi Access API - Door access control, user management, NFC/PIN credentials | 🚧 In Development | - |
| **Unifi.NET.Network** | SDK for UniFi Network Controller API - Network device management, configuration, statistics | 📋 Planned | - |
| **Unifi.NET.Protect** | SDK for UniFi Protect API - Video surveillance, camera management, event detection | 📋 Planned | - |
| **Unifi.NET.SiteManager** | SDK for UniFi Site Manager API - Multi-site management, centralized control | 📋 Planned | - |

## 🚀 Features

- **Native AOT Compatible** - Full support for Native AOT compilation for optimal performance and size
- **Strongly Typed Models** - Full request/response models for all API endpoints
- **Async/Await Support** - Built for modern async programming patterns
- **Dependency Injection** - Native integration with Microsoft.Extensions.DependencyInjection
- **Resilience & Retry Logic** - Built-in Polly policies for transient fault handling
- **RestSharp Based** - Leveraging RestSharp for robust HTTP client functionality
- **API Token Authentication** - Secure authentication using UniFi API tokens
- **Comprehensive Error Handling** - Detailed exception types and error codes
- **IntelliSense Support** - Full XML documentation for all public APIs
- **Trimming Safe** - Optimized for trimming to reduce application size

## 📋 Prerequisites

- .NET 9.0 or later
- UniFi Console with the respective service installed
- API Token from UniFi Portal (for authentication)
- Network access to your UniFi Console (default port: 12445 for Access)

### Native AOT Requirements
- .NET 9 SDK for building
- Target platform runtime identifier (e.g., `linux-x64`, `win-x64`, `osx-arm64`)
- No additional runtime dependencies needed for deployment

## 🔧 Installation

Install the SDK for the UniFi service you need via NuGet:

```bash
# For UniFi Access
dotnet add package Unifi.NET.Access

# For UniFi Network
dotnet add package Unifi.NET.Network

# For UniFi Protect
dotnet add package Unifi.NET.Protect

# For UniFi Site Manager
dotnet add package Unifi.NET.SiteManager
```

## 🎯 Quick Start

### Configuration with Dependency Injection

```csharp
using Microsoft.Extensions.DependencyInjection;
using Unifi.NET.Access;

var services = new ServiceCollection();

// Configure UniFi SDKs
services.AddUnifi(options =>
{
    options.BaseUrl = "https://your-console-ip:12445";
    options.ApiToken = "your-api-token-here";
    options.ValidateSsl = false; // Set to true in production with valid certificates
})
.AddAccess() // Add UniFi Access SDK
.AddNetwork() // Add UniFi Network SDK
.AddProtect(); // Add UniFi Protect SDK

var serviceProvider = services.BuildServiceProvider();
```

### Basic Usage Example

```csharp
// Resolve the Access client from DI
var accessClient = serviceProvider.GetRequiredService<IUnifiAccessClient>();

// Get all users
var users = await accessClient.Users.GetAllAsync();

// Get a specific door
var door = await accessClient.Doors.GetAsync("door-id");

// Unlock a door remotely
await accessClient.Doors.UnlockAsync("door-id");

// Create a new user
var newUser = await accessClient.Users.CreateAsync(new CreateUserRequest
{
    FirstName = "John",
    LastName = "Doe",
    Email = "john.doe@example.com",
    EmployeeNumber = "EMP001"
});

// Assign an access policy to a user
await accessClient.Users.AssignAccessPolicyAsync(newUser.Id, "policy-id");
```

### Without Dependency Injection

```csharp
using Unifi.NET.Access;

var config = new UnifiConfiguration
{
    BaseUrl = "https://your-console-ip:12445",
    ApiToken = "your-api-token-here"
};

var accessClient = new UnifiAccessClient(config);

// Use the client
var users = await accessClient.Users.GetAllAsync();
```

### Native AOT Deployment

The SDKs are designed for full Native AOT compatibility in .NET 9:

```xml
<!-- In your project file -->
<PropertyGroup>
    <PublishAot>true</PublishAot>
    <JsonSerializerIsReflectionEnabledByDefault>false</JsonSerializerIsReflectionEnabledByDefault>
    <IsAotCompatible>true</IsAotCompatible>
    <EnableConfigurationBindingGenerator>true</EnableConfigurationBindingGenerator>
    <OptimizationPreference>Speed</OptimizationPreference>
</PropertyGroup>
```

The SDK uses ASP.NET Core-style JSON serialization patterns for optimal Native AOT performance:

```csharp
// Internal implementation uses service-specific JsonSerializerContext classes
// Combined with TypeInfoResolver for runtime type resolution
// Following Microsoft's recommended patterns for Native AOT applications

// All JSON serialization is handled internally - no configuration needed!
services.AddUnifi(options =>
{
    options.BaseUrl = "https://your-console-ip:12445";
    options.ApiToken = "your-api-token-here";
})
.AddAccess(); // Serialization is automatically configured for AOT
```

```bash
# Publish with Native AOT
dotnet publish -c Release -r linux-x64

# Verify AOT compatibility (produces zero warnings)
dotnet publish -c Release -r linux-x64 --verbosity detailed | grep -i warning
```

## 🔑 Authentication

All UniFi SDKs use API Token authentication. To obtain an API token:

1. Sign in to your [UniFi Portal](https://account.ui.com/login)
2. Select the UniFi Console where the service is installed
3. Navigate to the respective service settings:
   - **Access**: Access > Settings > General > Advanced > API Token
   - **Network**: Settings > System > Advanced > API
   - **Protect**: Settings > Advanced > API
4. Create a new API token with appropriate permissions
5. Copy and securely store the token (it's only shown once)

## 📊 Supported UniFi Versions

| Service | Minimum Version | Recommended Version |
|---------|----------------|-------------------|
| UniFi Access | 1.9.1 | Latest |
| UniFi Network Controller | 7.0.0 | Latest |
| UniFi Protect | 2.0.0 | Latest |
| UniFi Site Manager | 1.0.0 | Latest |

> **Note**: API availability may vary based on your UniFi service version and license type. Identity Enterprise users should check API availability for their specific configuration.

## 🏗️ Architecture

The solution follows Clean Architecture principles with a modular design:

```
Unifi.NET/
├── Unifi.NET.Access/           # UniFi Access SDK
│   ├── Models/                 # Request/Response DTOs
│   ├── Services/               # API service implementations
│   ├── Extensions/             # DI extensions
│   └── Exceptions/             # Custom exceptions
├── Unifi.NET.Network/          # UniFi Network SDK
├── Unifi.NET.Protect/          # UniFi Protect SDK
├── Unifi.NET.SiteManager/      # UniFi Site Manager SDK
└── Unifi.NET.Common/           # Shared utilities (planned)
```

### Native AOT Design Principles

All SDKs follow .NET 9 Native AOT best practices:

#### ✅ **Fully Compatible Components**
- **System.Text.Json** with source generators (no reflection)
- **Microsoft.Extensions.DependencyInjection** 
- **Microsoft.Extensions.Http** with IHttpClientFactory
- **Polly v8+** for resilience (via Microsoft.Extensions.Http.Resilience)

#### 🚫 **Avoided Patterns**
- No runtime reflection for serialization
- No dynamic code generation (System.Reflection.Emit)
- No runtime type discovery
- No dynamic assembly loading

#### 🎯 **Implementation Strategy**
- Service-specific `JsonSerializerContext` classes for organized type registration
- `JsonTypeInfoResolver.Combine()` pattern for merging contexts (ASP.NET Core style)
- `JsonTypeInfo<T>` and `GetTypeInfo()` for runtime type resolution
- All models use `[JsonSerializable]` source generation
- Configuration uses source-generated binding
- RestSharp configured with System.Text.Json and AOT-safe serializers
- Zero AOT warnings policy enforced

## 🧪 Testing

Each SDK includes comprehensive unit and integration tests:

```bash
# Run all tests
dotnet test

# Run tests for a specific SDK
dotnet test Unifi.NET.Access.Tests

# Run with coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

## 📈 Versioning

Package versions follow the UniFi API version they wrap, with an additional revision suffix for SDK updates:

- `3.3.21` - Matches UniFi Access API v3.3.21, first SDK release
- `3.3.21.1` - Same API version, SDK bug fixes or improvements
- `3.4.0` - New UniFi Access API version

## 🤝 Contributing

We welcome contributions! Please see our [Contributing Guidelines](CONTRIBUTING.md) for details on:

- Code style and standards
- Pull request process
- Bug reporting
- Feature requests
- Development setup

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Support

- **Issues**: [GitHub Issues](https://github.com/yourusername/Unifi.NET/issues)
- **Discussions**: [GitHub Discussions](https://github.com/yourusername/Unifi.NET/discussions)
- **Documentation**: [Wiki](https://github.com/yourusername/Unifi.NET/wiki)

## 🗺️ Roadmap

### Phase 1: UniFi Access SDK (Current)
- [x] Project structure setup
- [ ] Native AOT-compatible HTTP client with RestSharp
- [ ] System.Text.Json source generators for all models
- [ ] User management endpoints
- [ ] Door and access control endpoints
- [ ] NFC/PIN credential management
- [ ] Access policy management
- [ ] Webhook support with real-time events
- [ ] Polly resilience policies
- [ ] Comprehensive testing with AOT verification
- [ ] Documentation and samples

### Phase 2: UniFi Network SDK
- [ ] Device management
- [ ] Network configuration
- [ ] Statistics and monitoring
- [ ] Guest management

### Phase 3: UniFi Protect SDK
- [ ] Camera management
- [ ] Recording and playback
- [ ] Event detection
- [ ] Live streaming support

### Phase 4: UniFi Site Manager SDK
- [ ] Multi-site management
- [ ] Centralized configuration
- [ ] Cross-site operations

## 🙏 Acknowledgments

- Ubiquiti Networks for creating the UniFi ecosystem
- The .NET community for continuous support and feedback
- Contributors and users of this project

## ⚠️ Disclaimer

This is an unofficial SDK and is not affiliated with, officially maintained, or endorsed by Ubiquiti Networks. Use at your own risk.

---

**Note**: This project is currently in active development. APIs may change before the first stable release. We recommend using preview versions with caution in production environments.