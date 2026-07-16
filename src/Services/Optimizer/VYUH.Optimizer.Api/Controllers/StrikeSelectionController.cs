using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using VYUH.Optimizer.Application.Queries;

namespace VYUH.Optimizer.Api.Controllers;

[ApiController]
[Route("api/v1/strike")]
public class StrikeSelectionController : ControllerBase
{
    private readonly IMediator _mediator;

    public StrikeSelectionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("select/{stockId}")]
    public async Task<IActionResult> SelectStrikes(string stockId, [FromQuery] string expiryDate = "")
    {
        if (string.IsNullOrEmpty(expiryDate))
        {
            expiryDate = DateTime.UtcNow.AddDays(15).ToString("yyyy-MM-dd");
        }
        var query = new SelectOptimalStrikesQuery(stockId, expiryDate);
        var result = await _mediator.Send(query);
        if (result == null)
        {
            return NotFound($"Unable to select written legs strikes for stock {stockId}.");
        }
        return Ok(result);
    }
}
