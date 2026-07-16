using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VYUH.Optimizer.Application.Queries;

namespace VYUH.Optimizer.Api.Controllers;

[ApiController]
[Route("api/v1/analytics/probability")]
public class ProbabilityController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProbabilityController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("historical/{stockId}/{strikePrice}")]
    public async Task<IActionResult> GetHistoricalProbability(string stockId, double strikePrice, [FromQuery] string expiryDate = "")
    {
        if (string.IsNullOrEmpty(expiryDate))
        {
            expiryDate = DateTime.UtcNow.AddDays(15).ToString("yyyy-MM-dd");
        }
        var query = new CalculateHistoricalProbabilityQuery(stockId, strikePrice, expiryDate);
        var result = await _mediator.Send(query);
        if (result == null)
        {
            return NotFound($"Unable to calculate historical probability for {stockId} at strike {strikePrice}.");
        }
        return Ok(result);
    }
    [HttpGet("bsm/{stockId}/{strikePrice}")]
    public async Task<IActionResult> GetBsmProbability(string stockId, double strikePrice, [FromQuery] string expiryDate = "")
    {
        if (string.IsNullOrEmpty(expiryDate))
        {
            expiryDate = DateTime.UtcNow.AddDays(15).ToString("yyyy-MM-dd");
        }
        var query = new CalculateBsmProbabilityQuery(stockId, strikePrice, expiryDate);
        var result = await _mediator.Send(query);
        if (result == null)
        {
            return NotFound($"Unable to calculate BSM probability for {stockId} at strike {strikePrice}.");
        }
        return Ok(result);
    }
    [HttpGet("touch/{stockId}/{strikePrice}")]
    public async Task<IActionResult> GetTouchProbability(string stockId, double strikePrice, [FromQuery] string expiryDate = "")
    {
        if (string.IsNullOrEmpty(expiryDate))
        {
            expiryDate = DateTime.UtcNow.AddDays(15).ToString("yyyy-MM-dd");
        }
        var query = new CalculateTouchProbabilityQuery(stockId, strikePrice, expiryDate);
        var result = await _mediator.Send(query);
        if (result == null)
        {
            return NotFound($"Unable to calculate Touch probability for {stockId} at strike {strikePrice}.");
        }
        return Ok(result);
    }
    [HttpGet("range/{stockId}/{lowerStrike}/{upperStrike}")]
    public async Task<IActionResult> GetRangeProbability(string stockId, double lowerStrike, double upperStrike, [FromQuery] string expiryDate = "")
    {
        if (string.IsNullOrEmpty(expiryDate))
        {
            expiryDate = DateTime.UtcNow.AddDays(15).ToString("yyyy-MM-dd");
        }
        var query = new CalculateRangeProbabilityQuery(stockId, lowerStrike, upperStrike, expiryDate);
        var result = await _mediator.Send(query);
        if (result == null)
        {
            return NotFound($"Unable to calculate Range probability for {stockId} in range [{lowerStrike}, {upperStrike}].");
        }
        return Ok(result);
    }
}
