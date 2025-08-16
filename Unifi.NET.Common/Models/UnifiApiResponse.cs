using System.Text.Json.Serialization;

namespace Unifi.NET.Common.Models;

/// <summary>
/// Represents a standard UniFi API response.
/// </summary>
/// <typeparam name="T">The type of data contained in the response.</typeparam>
public class UnifiApiResponse<T>
{
    /// <summary>
    /// Gets or sets the response code.
    /// </summary>
    [JsonPropertyName("code")]
    public required string Code { get; set; }

    /// <summary>
    /// Gets or sets the response message.
    /// </summary>
    [JsonPropertyName("msg")]
    public required string Message { get; set; }

    /// <summary>
    /// Gets or sets the response data.
    /// </summary>
    [JsonPropertyName("data")]
    public T? Data { get; set; }

    /// <summary>
    /// Gets a value indicating whether the request was successful.
    /// </summary>
    [JsonIgnore]
    public bool IsSuccess => Code == "SUCCESS";
}