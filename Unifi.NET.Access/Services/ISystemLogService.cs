using Unifi.NET.Access.Models.SystemLogs;
using Unifi.NET.Access.Models;

namespace Unifi.NET.Access.Services;

/// <summary>
/// Service for managing system logs and audit trails in UniFi Access.
/// </summary>
public interface ISystemLogService
{
    /// <summary>
    /// Fetches system logs with filtering options.
    /// </summary>
    /// <param name="request">The system log query request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>System log response with matching log entries and pagination info.</returns>
    Task<SystemLogQueryResponse> GetSystemLogsAsync(SystemLogRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches system logs with pagination.
    /// </summary>
    /// <param name="request">The system log query request.</param>
    /// <param name="pageNum">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>System log response with matching log entries and pagination info.</returns>
    Task<SystemLogQueryResponse> GetSystemLogsAsync(SystemLogRequest request, int pageNum, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exports system logs to CSV format.
    /// </summary>
    /// <param name="request">The system log export request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>CSV data as byte array.</returns>
    Task<byte[]> ExportSystemLogsAsync(SystemLogExportRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches resources referenced in system logs.
    /// </summary>
    /// <param name="resourceId">The resource ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Resource information.</returns>
    Task<SystemLogResourceResponse> GetSystemLogResourceAsync(string resourceId, CancellationToken cancellationToken = default);
}