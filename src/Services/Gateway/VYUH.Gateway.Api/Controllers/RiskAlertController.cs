using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VYUH.Gateway.Api.Filters;
using VYUH.Gateway.Application;

namespace VYUH.Gateway.Api.Controllers;

[ApiController]
[Route("api/v1/gateway/risk")]
[Authorize]
[ServiceFilter(typeof(HmacSignatureFilter))]
[ServiceFilter(typeof(AuditLogFilter))]
public class RiskAlertController : ControllerBase
{
    private readonly IRiskAlertService _alertService;

    public RiskAlertController(IRiskAlertService alertService)
    {
        _alertService = alertService;
    }

    [HttpPost("trigger-breach")]
    public async Task<IActionResult> TriggerBreach([FromBody] BreachRequest request)
    {
        await _alertService.BroadcastBreachAsync(
            request.BreachType,
            request.StockId,
            request.CurrentMarginUsage,
            request.Limit,
            request.Severity
        );
        return Ok(new { message = "Breach broadcasted successfully." });
    }
}

public class BreachRequest
{
    public string BreachType { get; set; } = string.Empty;
    public string StockId { get; set; } = string.Empty;
    public decimal CurrentMarginUsage { get; set; }
    public decimal Limit { get; set; }
    public string Severity { get; set; } = string.Empty;
}
