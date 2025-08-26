namespace Unifi.NET.AccessViewer.Shared;

public class AccessLogRequest
{
    public string? UserId { get; set; }
    public long Since { get; set; }
    public long Until { get; set; }
    public int? PageNum { get; set; }
    public int? PageSize { get; set; }
}

public class AccessLogResponse
{
    public List<AccessLogEntry> Logs { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNum { get; set; }
}

public class AccessLogEntry
{
    public string Timestamp { get; set; } = string.Empty;
    public string ActorName { get; set; } = string.Empty;
    public string? EmployeeNumber { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string EventMessage { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public string AuthMethod { get; set; } = string.Empty;
    public string DoorName { get; set; } = string.Empty;
    public string? FloorName { get; set; }
    public string? BuildingName { get; set; }
}

public class UserSearchResult
{
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string? EmployeeNumber { get; set; }
    public string? Status { get; set; }
}