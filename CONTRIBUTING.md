# Contributing to Unifi.NET

Thank you for your interest in contributing to Unifi.NET! This guide will help you implement new API endpoints and maintain consistency across the SDK.

## 📋 Before You Start

1. **Check the tracking document**: Review [API_IMPLEMENTATION_STATUS.md](API_IMPLEMENTATION_STATUS.md) to see what needs implementation
2. **Read CLAUDE.md**: Understand the project architecture and conventions in [CLAUDE.md](CLAUDE.md)
3. **Verify Native AOT compatibility**: All code must be Native AOT compatible with zero warnings

## 🚀 Implementation Workflow

### Step 1: Choose an Endpoint

1. Check [API_IMPLEMENTATION_STATUS.md](API_IMPLEMENTATION_STATUS.md)
2. Start with **P0 (Core)** endpoints if available
3. Update the status to "In Progress" when you begin

### Step 2: Create Models

For each endpoint, create request/response models:

```csharp
using System.Text.Json.Serialization;

namespace Unifi.NET.Access.Models;

public class CreateUserRequest
{
    [JsonPropertyName("first_name")]
    public required string FirstName { get; set; }
    
    [JsonPropertyName("last_name")]
    public required string LastName { get; set; }
    
    [JsonPropertyName("email")]
    public string? Email { get; set; }
}
```

### Step 3: Register with JSON Context

Add all new models to the JSON source generator context:

```csharp
[JsonSerializable(typeof(CreateUserRequest))]
[JsonSerializable(typeof(UserResponse))]
[JsonSerializable(typeof(UnifiApiResponse<UserResponse>))]
internal partial class UnifiAccessJsonContext : JsonSerializerContext { }
```

### Step 4: Define Service Interface

Add method to the appropriate service interface:

```csharp
public interface IUserService
{
    /// <summary>
    /// Creates a new user in UniFi Access.
    /// </summary>
    /// <param name="request">The user creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created user.</returns>
    Task<UserResponse> CreateUserAsync(
        CreateUserRequest request, 
        CancellationToken cancellationToken = default);
}
```

### Step 5: Implement Service Method

```csharp
public async Task<UserResponse> CreateUserAsync(
    CreateUserRequest request, 
    CancellationToken cancellationToken = default)
{
    var restRequest = new RestRequest("/api/v1/users", Method.Post)
        .AddJsonBody(request);
    
    var response = await _restClient.ExecuteAsync<UnifiApiResponse<UserResponse>>(
        restRequest, 
        cancellationToken);
    
    HandleResponse(response);
    return response.Data!.Data!;
}
```

### Step 6: Write Tests

Create unit tests for each endpoint:

```csharp
[Fact]
public async Task CreateUserAsync_ShouldReturnUser_WhenRequestIsValid()
{
    // Arrange
    var request = new CreateUserRequest 
    { 
        FirstName = "John", 
        LastName = "Doe" 
    };
    
    // Act
    var result = await _service.CreateUserAsync(request);
    
    // Assert
    result.Should().NotBeNull();
    result.FirstName.Should().Be("John");
}
```

### Step 7: Update Documentation

1. Add XML documentation to all public methods
2. Update the endpoint's checkbox in [API_IMPLEMENTATION_STATUS.md](API_IMPLEMENTATION_STATUS.md)
3. Add usage examples if implementing a major feature

## 🔒 Native AOT Requirements

### DO ✅
- Use `[JsonSerializable]` attributes on all models
- Register models in `JsonSerializerContext`
- Use `required` properties where appropriate
- Test with `~/.dotnet/dotnet publish -c Release -r linux-x64`

### DON'T ❌
- Use reflection for serialization
- Use dynamic JSON parsing
- Create types at runtime
- Use `JsonSerializer` without context

## 🧪 Testing Guidelines

### Unit Tests
- Mock `RestClient` responses
- Test error handling
- Test parameter validation
- Test cancellation tokens

### Integration Tests (if applicable)
- Use test fixtures for shared setup
- Mock the UniFi API server
- Test authentication flows
- Test retry policies

## 📝 Code Style

- Follow the `.editorconfig` settings
- Use meaningful variable names
- Add XML documentation to public APIs
- Keep methods focused and small
- Use async/await consistently

## 🎯 Priority Guidelines

### P0 - Core (Implement First)
- Authentication mechanisms
- Basic user operations
- Door control
- Access policies

### P1 - Essential
- Credentials (NFC, PIN)
- User groups
- Door groups

### P2 - Extended
- Visitors
- Schedules
- Holiday groups

### P3 - Advanced
- WebSockets
- Webhooks
- Identity integration

## 📊 Tracking Your Progress

After implementing an endpoint:

1. ✅ Check off the endpoint in [API_IMPLEMENTATION_STATUS.md](API_IMPLEMENTATION_STATUS.md)
2. 📈 Update the progress percentages
3. 📝 Add any notes about special considerations
4. 🔗 Link to relevant PRs or issues

## 🐛 Reporting Issues

If you encounter blockers:

1. Document in [API_IMPLEMENTATION_STATUS.md](API_IMPLEMENTATION_STATUS.md)
2. Open a GitHub issue with details
3. Tag with appropriate labels (bug, help-wanted, etc.)

## 🤝 Pull Request Process

1. **Branch naming**: `feature/implement-{service}-{endpoint}`
2. **Commit messages**: Follow conventional commits
3. **PR description**: Reference the API section being implemented
4. **Tests**: All PRs must include tests
5. **AOT check**: Verify zero warnings with Native AOT compilation

## 📚 Resources

- [UniFi Access API Reference](Unifi.NET.Access/UniFi_Access_API_Reference.md)
- [CLAUDE.md](CLAUDE.md) - Project architecture
- [API Implementation Status](API_IMPLEMENTATION_STATUS.md) - Progress tracking
- [Microsoft Native AOT Documentation](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/)

## ❓ Questions?

- Check existing issues first
- Ask in discussions for design questions
- Tag maintainers for urgent issues

Thank you for contributing to Unifi.NET! 🎉