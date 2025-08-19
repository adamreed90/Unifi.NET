namespace Unifi.NET.Access.Exceptions;

/// <summary>
/// Base exception for UniFi Access API errors.
/// </summary>
public class UnifiAccessException : Exception
{
    /// <summary>
    /// Error code returned by the API.
    /// </summary>
    public string ErrorCode { get; }

    /// <summary>
    /// HTTP status code (if applicable).
    /// </summary>
    public int? StatusCode { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnifiAccessException"/> class.
    /// </summary>
    public UnifiAccessException(string message, string errorCode, int? statusCode = null) 
        : base(message)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnifiAccessException"/> class.
    /// </summary>
    public UnifiAccessException(string message, string errorCode, Exception innerException, int? statusCode = null) 
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }
}

/// <summary>
/// Exception thrown when authentication fails.
/// </summary>
public sealed class UnifiAuthenticationException : UnifiAccessException
{
    public UnifiAuthenticationException(string message, string errorCode, int? statusCode = null) 
        : base(message, errorCode, statusCode) { }
}

/// <summary>
/// Exception thrown when a resource is not found.
/// </summary>
public sealed class UnifiNotFoundException : UnifiAccessException
{
    public UnifiNotFoundException(string message, string errorCode, int? statusCode = null) 
        : base(message, errorCode, statusCode) { }
}

/// <summary>
/// Exception thrown when parameters are invalid.
/// </summary>
public sealed class UnifiValidationException : UnifiAccessException
{
    public UnifiValidationException(string message, string errorCode, int? statusCode = null) 
        : base(message, errorCode, statusCode) { }
}

/// <summary>
/// Exception thrown when an operation is forbidden.
/// </summary>
public sealed class UnifiForbiddenException : UnifiAccessException
{
    public UnifiForbiddenException(string message, string errorCode, int? statusCode = null) 
        : base(message, errorCode, statusCode) { }
}

/// <summary>
/// Exception thrown when rate limiting is encountered.
/// </summary>
public sealed class UnifiRateLimitException : UnifiAccessException
{
    public UnifiRateLimitException(string message, string errorCode, int? statusCode = null) 
        : base(message, errorCode, statusCode) { }
}

/// <summary>
/// Maps UniFi error codes to appropriate exceptions.
/// </summary>
public static class UnifiErrorCodeMapper
{
    /// <summary>
    /// Maps an error code and message to the appropriate exception.
    /// </summary>
    public static UnifiAccessException MapError(string errorCode, string message, int? statusCode = null)
    {
        // Include error code in the message for better debugging
        var fullMessage = string.IsNullOrEmpty(errorCode) || errorCode == "CODE_SYSTEM_ERROR" 
            ? message 
            : $"{message} (Error Code: {errorCode})";
        
        return errorCode switch
        {
            "CODE_AUTH_FAILED" or "CODE_ACCESS_TOKEN_INVALID" or "CODE_UNAUTHORIZED" 
                => new UnifiAuthenticationException(fullMessage, errorCode, statusCode),
            
            "CODE_RESOURCE_NOT_FOUND" or "CODE_NOT_EXISTS" or "CODE_USER_ACCOUNT_NOT_EXIST" or 
            "CODE_USER_WORKER_NOT_EXISTS" or "CODE_DEVICE_DEVICE_NOT_FOUND"
                => new UnifiNotFoundException(fullMessage, errorCode, statusCode),
            
            "CODE_PARAMS_INVALID" or "CODE_USER_EMAIL_ERROR" or "CODE_USER_NAME_DUPLICATED" or
            "CODE_CREDS_PIN_CODE_CREDS_LENGTH_INVALID"
                => new UnifiValidationException(fullMessage, errorCode, statusCode),
            
            "CODE_OPERATION_FORBIDDEN" or "CODE_DEVICE_DEVICE_BUSY" or "CODE_DEVICE_DEVICE_OFFLINE"
                => new UnifiForbiddenException(fullMessage, errorCode, statusCode),
            
            _ => new UnifiAccessException(fullMessage, errorCode, statusCode)
        };
    }
}