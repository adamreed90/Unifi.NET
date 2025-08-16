using System.Text.Json.Serialization;

namespace Unifi.NET.Access.Models;

/// <summary>
/// Generic wrapper for UniFi API responses.
/// </summary>
/// <typeparam name="T">The type of data returned in the response.</typeparam>
public sealed class UnifiApiResponse<T>
{
    /// <summary>
    /// Response code (SUCCESS or error code).
    /// </summary>
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Response message.
    /// </summary>
    [JsonPropertyName("msg")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Response data.
    /// </summary>
    [JsonPropertyName("data")]
    public T? Data { get; set; }

    /// <summary>
    /// Pagination information (when applicable).
    /// </summary>
    [JsonPropertyName("pagination")]
    public PaginationInfo? Pagination { get; set; }
}

/// <summary>
/// Pagination information.
/// </summary>
public sealed class PaginationInfo
{
    /// <summary>
    /// Current page number.
    /// </summary>
    [JsonPropertyName("page_num")]
    public int PageNum { get; set; }

    /// <summary>
    /// Number of items per page.
    /// </summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of items.
    /// </summary>
    [JsonPropertyName("total")]
    public int Total { get; set; }
}