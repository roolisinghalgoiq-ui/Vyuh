using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VYUH.Risk.Application.Queries;

namespace VYUH.Risk.Api.Controllers;

[ApiController]
[Route("api/v1/risk/margin")]
public class MarginController : ControllerBase
{
    private readonly IMediator _mediator;

    public MarginController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("estimate")]
    public async Task<IActionResult> EstimateMargin([FromBody] EstimateMarginQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
