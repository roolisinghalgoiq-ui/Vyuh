using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VYUH.Ingestion.Application.Commands;

namespace VYUH.Ingestion.Api.Controllers;

[ApiController]
[Route("api/v1/ingestion/greeks")]
public class GreeksController : ControllerBase
{
    private readonly IMediator _mediator;

    public GreeksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("fetch/{stockId}/{expiryDate}")]
    public async Task<IActionResult> FetchGreeks(string stockId, string expiryDate)
    {
        var command = new FetchOptionGreeksCommand(stockId, expiryDate);
        var result = await _mediator.Send(command);
        if (result == null)
        {
            return NotFound($"Option Greeks could not be generated or merged for {stockId} expiring on {expiryDate}.");
        }
        return Ok(result);
    }
}
