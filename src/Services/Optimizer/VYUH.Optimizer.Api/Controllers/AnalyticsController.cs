using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VYUH.Optimizer.Application.Queries;

namespace VYUH.Optimizer.Api.Controllers;

[ApiController]
[Route("api/v1/analytics")]
public class AnalyticsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AnalyticsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("expected-move/{stockId}")]
    public async Task<IActionResult> GetExpectedMove(string stockId, [FromQuery] string expiryDate = "")
    {
        if (string.IsNullOrEmpty(expiryDate))
        {
            expiryDate = DateTime.UtcNow.AddDays(15).ToString("yyyy-MM-dd");
        }
        var query = new CalculateExpectedMoveQuery(stockId, expiryDate);
        var result = await _mediator.Send(query);
        if (result == null)
        {
            return NotFound($"Unable to calculate expected move for {stockId}.");
        }
        return Ok(result);
    }
}
