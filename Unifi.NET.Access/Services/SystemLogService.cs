using RestSharp;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Unifi.NET.Access.Configuration;
using Unifi.NET.Access.Exceptions;
using Unifi.NET.Access.Models;
using Unifi.NET.Access.Models.SystemLogs;
using Unifi.NET.Access.Serialization;

namespace Unifi.NET.Access.Services;

/// <summary>
/// Service implementation for managing system logs and audit trails in UniFi Access.
/// </summary>
public sealed class SystemLogService : BaseService, ISystemLogService
{
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemLogService"/> class.
    /// </summary>
    public SystemLogService(RestClient client, UnifiAccessConfiguration configuration) 
        : base(client, configuration)
    {
        _jsonOptions = UnifiAccessJsonContext.CreateOptions();
    }

    /// <inheritdoc />
    public async Task<SystemLogResponse> GetSystemLogsAsync(
        SystemLogRequest request,
        CancellationToken cancellationToken = default)
    {
        return await GetSystemLogsAsync(request, 1, 25, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<SystemLogResponse> GetSystemLogsAsync(
        SystemLogRequest request,
        int pageNum,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        if (pageNum < 1)
        {
            throw new ArgumentException("Page number must be greater than 0", nameof(pageNum));
        }
        if (pageSize < 1 || pageSize > 250)
        {
            throw new ArgumentException("Page size must be between 1 and 250", nameof(pageSize));
        }

        var apiRequest = CreateRequest("/api/v1/developer/system/logs", Method.Post);
        apiRequest.AddQueryParameter("page_num", pageNum.ToString());
        apiRequest.AddQueryParameter("page_size", pageSize.ToString());
        apiRequest.AddJsonBody(request);

        var response = await Client.ExecuteAsync(apiRequest, cancellationToken);

        if (!response.IsSuccessful)
        {
            throw new UnifiAccessException($"Failed to fetch system logs: {response.StatusCode}", "CODE_SYSTEM_ERROR", (int?)response.StatusCode);
        }

        // Use source-generated JSON deserialization
        var jsonTypeInfo = (JsonTypeInfo<UnifiApiResponse<SystemLogResponse>>)_jsonOptions.GetTypeInfo(typeof(UnifiApiResponse<SystemLogResponse>));
        var result = JsonSerializer.Deserialize(response.Content ?? string.Empty, jsonTypeInfo);
        
        if (result?.Data == null)
        {
            throw new UnifiAccessException("Invalid response from API", "NULL_RESPONSE");
        }

        return result.Data;
    }

    /// <inheritdoc />
    public async Task<byte[]> ExportSystemLogsAsync(
        SystemLogExportRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Since >= request.Until)
        {
            throw new ArgumentException("Since must be before Until", nameof(request));
        }
        
        // Check that date range doesn't exceed one month
        var dateRange = request.Until - request.Since;
        const long oneMonthInSeconds = 30 * 24 * 60 * 60; // Approximately 30 days
        if (dateRange > oneMonthInSeconds)
        {
            throw new ArgumentException("Date range cannot exceed one month", nameof(request));
        }

        var apiRequest = CreateRequest("/api/v1/developer/system/logs/export", Method.Post);
        apiRequest.AddJsonBody(request);

        var response = await Client.ExecuteAsync(apiRequest, cancellationToken);

        if (!response.IsSuccessful)
        {
            throw new UnifiAccessException($"Failed to export system logs: {response.StatusCode}", "CODE_SYSTEM_ERROR", (int?)response.StatusCode);
        }

        return response.RawBytes ?? Array.Empty<byte>();
    }

    /// <inheritdoc />
    public async Task<SystemLogResourceResponse> GetSystemLogResourceAsync(
        string resourceId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(resourceId);

        return await GetAsync<SystemLogResourceResponse>($"/api/v1/developer/system/logs/resource/{resourceId}", cancellationToken);
    }
}