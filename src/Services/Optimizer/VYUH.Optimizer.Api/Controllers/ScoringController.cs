using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using VYUH.Optimizer.Application.Commands;

namespace VYUH.Optimizer.Api.Controllers;

[ApiController]
[Route("api/v1/scoring")]
public class ScoringController : ControllerBase
{
    private readonly IMediator _mediator;

    public ScoringController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("calculate/{stockId}")]
    public async Task<IActionResult> CalculateStockScore(string stockId, [FromQuery] string expiryDate = "")
    {
        if (string.IsNullOrEmpty(expiryDate))
        {
            expiryDate = DateTime.UtcNow.AddDays(15).ToString("yyyy-MM-dd");
        }
        var command = new CalculateStockScoresCommand(stockId, expiryDate);
        var score = await _mediator.Send(command);
        return Ok(new { StockId = stockId, OverallStockScore = score });
    }
}
