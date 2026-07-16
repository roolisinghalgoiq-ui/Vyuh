using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VYUH.Gateway.Application.Commands;

using Microsoft.AspNetCore.Authorization;
using VYUH.Gateway.Api.Filters;

namespace VYUH.Gateway.Api.Controllers;

[ApiController]
[Route("api/v1/gateway/order")]
[Authorize]
[ServiceFilter(typeof(HmacSignatureFilter))]
[ServiceFilter(typeof(AuditLogFilter))]
public class OrderController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrderController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("submit")]
    public async Task<IActionResult> SubmitOrder([FromBody] SubmitOrderCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
