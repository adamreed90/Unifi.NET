namespace Unifi.NET.Common.Exceptions;

/// <summary>
/// Base exception for UniFi API errors.
/// </summary>
public class UnifiException : Exception
{
    /// <summary>
    /// Gets the error code returned by the UniFi API.
    /// </summary>
    public string? ErrorCode { get; }

    /// <summary>
    /// Gets the HTTP status code of the response.
    /// </summary>
    public int? StatusCode { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnifiException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public UnifiException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnifiException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public UnifiException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnifiException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="errorCode">The error code from the API.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    public UnifiException(string message, string errorCode, int statusCode) : base(message)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }
}

/// <summary>
/// Exception thrown when authentication fails.
/// </summary>
public class UnifiAuthenticationException : UnifiException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnifiAuthenticationException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public UnifiAuthenticationException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnifiAuthenticationException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="errorCode">The error code from the API.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    public UnifiAuthenticationException(string message, string errorCode, int statusCode) 
        : base(message, errorCode, statusCode)
    {
    }
}

/// <summary>
/// Exception thrown when a requested resource is not found.
/// </summary>
public class UnifiNotFoundException : UnifiException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnifiNotFoundException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public UnifiNotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnifiNotFoundException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="errorCode">The error code from the API.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    public UnifiNotFoundException(string message, string errorCode, int statusCode) 
        : base(message, errorCode, statusCode)
    {
    }
}