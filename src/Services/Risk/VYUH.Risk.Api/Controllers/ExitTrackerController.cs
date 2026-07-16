using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VYUH.Risk.Application.Queries;

namespace VYUH.Risk.Api.Controllers;

[ApiController]
[Route("api/v1/risk/exits")]
public class ExitTrackerController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExitTrackerController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("evaluate")]
    public async Task<IActionResult> EvaluateExits([FromBody] EvaluatePositionExitsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
