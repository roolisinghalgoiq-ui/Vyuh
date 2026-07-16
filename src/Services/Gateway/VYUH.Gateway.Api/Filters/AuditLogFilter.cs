using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using VYUH.Gateway.Application;
using VYUH.Gateway.Domain;

namespace VYUH.Gateway.Api.Filters;

public class AuditLogFilter : IAsyncActionFilter
{
    private readonly IUserAuditLogRepository _auditLogRepository;

    public AuditLogFilter(IUserAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var request = context.HttpContext.Request;

        request.EnableBuffering();
        request.Body.Position = 0;
        using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
        var requestDetails = await reader.ReadToEndAsync();
        request.Body.Position = 0;

        var username = context.HttpContext.User.Identity?.Name ?? "Anonymous";
        var actionName = context.ActionDescriptor.DisplayName ?? request.Path;
        var ipAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        var executedContext = await next();

        var responseStatus = executedContext.HttpContext.Response.StatusCode;

        var auditLog = new UserAuditLog
        {
            AuditLogId = Guid.NewGuid(),
            Username = username,
            Action = actionName,
            Timestamp = DateTime.UtcNow,
            IpAddress = ipAddress,
            RequestDetails = requestDetails,
            ResponseStatus = responseStatus
        };

        try
        {
            await _auditLogRepository.SaveAuditLogAsync(auditLog);
        }
        catch
        {
            // Prevent database audit failure from failing the main request
        }
    }
}
