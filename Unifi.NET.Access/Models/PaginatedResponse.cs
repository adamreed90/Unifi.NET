using System.Text.Json.Serialization;

namespace Unifi.NET.Access.Models;

/// <summary>
/// Generic paginated response wrapper for API responses that include pagination information.
/// </summary>
/// <typeparam name="T">The type of data in the response.</typeparam>
public sealed class PaginatedResponse<T>
{
    /// <summary>
    /// The data items for the current page.
    /// </summary>
    public T Items { get; set; } = default!;

    /// <summary>
    /// Current page number (1-based).
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Number of items per page.
    /// </summary>
    public int PageSize { get; set; } = 25;

    /// <summary>
    /// Total number of items across all pages.
    /// </summary>
    public int Total { get; set; } = 0;

    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    [JsonIgnore]
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)Total / PageSize) : 0;

    /// <summary>
    /// Gets whether there is a next page available.
    /// </summary>
    [JsonIgnore]
    public bool HasNextPage => Page < TotalPages;

    /// <summary>
    /// Gets whether there is a previous page available.
    /// </summary>
    [JsonIgnore]
    public bool HasPreviousPage => Page > 1;
}