using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VYUH.Gateway.Application.Queries;

using Microsoft.AspNetCore.Authorization;
using VYUH.Gateway.Api.Filters;

namespace VYUH.Gateway.Api.Controllers;

[ApiController]
[Route("api/v1/gateway/reconciliation")]
[Authorize]
[ServiceFilter(typeof(HmacSignatureFilter))]
[ServiceFilter(typeof(AuditLogFilter))]
public class ReconciliationController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReconciliationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("reconcile")]
    public async Task<IActionResult> Reconcile([FromBody] ReconcilePositionsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
