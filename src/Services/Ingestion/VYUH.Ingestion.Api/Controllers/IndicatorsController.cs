using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VYUH.Ingestion.Application.Queries;

namespace VYUH.Ingestion.Api.Controllers;

[ApiController]
[Route("api/v1/indicators")]
public class IndicatorsController : ControllerBase
{
    private readonly IMediator _mediator;

    public IndicatorsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{stockId}")]
    public async Task<IActionResult> GetTechnicalIndicators(string stockId)
    {
        var query = new GetTechnicalIndicatorsQuery(stockId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
