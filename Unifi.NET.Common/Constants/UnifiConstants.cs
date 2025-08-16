namespace Unifi.NET.Common.Constants;

/// <summary>
/// Common constants used across UniFi SDKs.
/// </summary>
public static class UnifiConstants
{
    /// <summary>
    /// Success response code from UniFi API.
    /// </summary>
    public const string SuccessCode = "SUCCESS";

    /// <summary>
    /// Common error codes from UniFi API.
    /// </summary>
    public static class ErrorCodes
    {
        public const string ParamsInvalid = "CODE_PARAMS_INVALID";
        public const string SystemError = "CODE_SYSTEM_ERROR";
        public const string ResourceNotFound = "CODE_RESOURCE_NOT_FOUND";
        public const string OperationForbidden = "CODE_OPERATION_FORBIDDEN";
        public const string AuthFailed = "CODE_AUTH_FAILED";
        public const string AccessTokenInvalid = "CODE_ACCESS_TOKEN_INVALID";
        public const string Unauthorized = "CODE_UNAUTHORIZED";
        public const string NotExists = "CODE_NOT_EXISTS";
    }

    /// <summary>
    /// HTTP headers used in UniFi API requests.
    /// </summary>
    public static class Headers
    {
        public const string Authorization = "Authorization";
        public const string ContentType = "Content-Type";
        public const string Accept = "Accept";
    }

    /// <summary>
    /// Content types for API requests.
    /// </summary>
    public static class ContentTypes
    {
        public const string Json = "application/json";
    }
}