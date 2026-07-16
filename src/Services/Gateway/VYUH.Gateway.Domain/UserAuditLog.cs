using System;

namespace VYUH.Gateway.Domain;

public class UserAuditLog
{
    public Guid AuditLogId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string RequestDetails { get; set; } = string.Empty;
    public int ResponseStatus { get; set; }
}
