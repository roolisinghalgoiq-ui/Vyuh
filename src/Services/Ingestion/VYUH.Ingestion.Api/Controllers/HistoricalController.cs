using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VYUH.Ingestion.Application.Queries;

namespace VYUH.Ingestion.Api.Controllers;

[ApiController]
[Route("api/v1/historical")]
public class HistoricalController : ControllerBase
{
    private readonly IMediator _mediator;

    public HistoricalController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("returns/{stockId}")]
    public async Task<IActionResult> GetHistoricalReturns(string stockId, [FromQuery] int days = 252)
    {
        var query = new GetHistoricalReturnsQuery(stockId, days);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
