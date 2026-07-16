using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using VYUH.Optimizer.Application.Queries;

namespace VYUH.Optimizer.Api.Controllers;

[ApiController]
[Route("api/v1/filtering")]
public class FilteringController : ControllerBase
{
    private readonly IMediator _mediator;

    public FilteringController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("run/{stockId}")]
    public async Task<IActionResult> RunFilteringPipeline(string stockId, [FromQuery] string expiryDate = "")
    {
        if (string.IsNullOrEmpty(expiryDate))
        {
            expiryDate = DateTime.UtcNow.AddDays(15).ToString("yyyy-MM-dd");
        }
        var query = new RunFilteringPipelineQuery(stockId, expiryDate);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
