using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VYUH.Ingestion.Application.Commands;

namespace VYUH.Ingestion.Api.Controllers;

[ApiController]
[Route("api/v1/ingestion/liquidity")]
public class LiquidityController : ControllerBase
{
    private readonly IMediator _mediator;

    public LiquidityController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("score/{stockId}/{expiryDate}")]
    public async Task<IActionResult> CalculateLiquidity(string stockId, string expiryDate)
    {
        var command = new CalculateLiquidityScoresCommand(stockId, expiryDate);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
