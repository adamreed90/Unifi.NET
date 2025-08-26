using Unifi.NET.Access;
using Unifi.NET.Access.Configuration;
using Unifi.NET.Access.Models.SystemLogs;
using Unifi.NET.AccessViewer.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient",
        policy =>
        {
            policy.WithOrigins("https://localhost:7000", "http://localhost:5000", "http://localhost:5279", "https://localhost:7186")
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

builder.Services.AddOpenApi();

// Configure UniFi Access client
builder.Services.AddSingleton<IUnifiAccessClient>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var baseUrl = configuration["UnifiAccess:BaseUrl"];
    var apiToken = configuration["UnifiAccess:ApiToken"];
    
    if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(apiToken))
    {
        throw new InvalidOperationException("UniFi Access configuration is missing. Please configure BaseUrl and ApiToken in appsettings.json or environment variables.");
    }
    
    var config = new UnifiAccessConfiguration
    {
        BaseUrl = baseUrl,
        ApiToken = apiToken,
        ValidateSslCertificate = configuration.GetValue<bool>("UnifiAccess:ValidateSslCertificate"),
        TimeoutSeconds = configuration.GetValue<int?>("UnifiAccess:TimeoutSeconds") ?? 30
    };
    
    return new UnifiAccessClient(config);
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseCors("AllowBlazorClient");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Map API endpoints
app.MapGet("/api/users/search", async (string? employeeNumber, IUnifiAccessClient client) =>
{
    if (string.IsNullOrWhiteSpace(employeeNumber))
    {
        return Results.BadRequest("Employee number is required");
    }

    try
    {
        // Use search API for much faster response
        var users = await client.Users.SearchUsersAsync(employeeNumber);
        var user = users.FirstOrDefault(u => u.EmployeeNumber == employeeNumber);
        
        if (user == null)
        {
            return Results.NotFound($"No user found with employee number: {employeeNumber}");
        }

        return Results.Ok(new UserSearchResult
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = user.FullName,
            EmployeeNumber = user.EmployeeNumber,
            Status = user.Status
        });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error searching users: {ex.Message}");
    }
})
.WithName("SearchUser");

app.MapPost("/api/access-logs", async (AccessLogRequest request, IUnifiAccessClient client) =>
{
    try
    {
        var logRequest = new SystemLogRequest
        {
            Topic = SystemLogTopic.DoorOpenings,
            Since = request.Since / 1000, // Convert milliseconds to seconds
            Until = request.Until / 1000, // Convert milliseconds to seconds
            ActorId = request.UserId
        };

        var response = await client.SystemLogs.GetSystemLogsAsync(
            logRequest, 
            request.PageNum ?? 1, 
            request.PageSize ?? 50);

        var logs = response.Hits.Select(hit => new AccessLogEntry
        {
            Timestamp = hit.Timestamp,
            ActorName = hit.Source.Actor.DisplayName,
            EmployeeNumber = hit.Source.Actor.AlternateId,
            EventType = hit.Source.Event.Type,
            EventMessage = hit.Source.Event.DisplayMessage,
            Result = hit.Source.Event.Result,
            AuthMethod = hit.Source.Authentication?.CredentialProvider ?? "Unknown",
            DoorName = hit.Source.Target?.FirstOrDefault(t => t.Type == "door")?.DisplayName ?? "Unknown",
            FloorName = hit.Source.Target?.FirstOrDefault(t => t.Type == "floor")?.DisplayName,
            BuildingName = hit.Source.Target?.FirstOrDefault(t => t.Type == "building")?.DisplayName
        }).ToList();

        return Results.Ok(new AccessLogResponse
        {
            Logs = logs,
            TotalCount = response.Total,
            PageNum = response.Page
        });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error fetching access logs: {ex.Message}");
    }
})
.WithName("GetAccessLogs");

app.Run();