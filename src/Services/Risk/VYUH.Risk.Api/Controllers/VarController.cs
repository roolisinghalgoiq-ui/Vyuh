using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VYUH.Risk.Application.Queries;

namespace VYUH.Risk.Api.Controllers;

[ApiController]
[Route("api/v1/risk/var")]
public class VarController : ControllerBase
{
    private readonly IMediator _mediator;

    public VarController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("calculate")]
    public async Task<IActionResult> CalculateVar([FromBody] CalculateVarQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
