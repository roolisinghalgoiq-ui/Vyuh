using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VYUH.Ingestion.Application;

namespace VYUH.Ingestion.Api.Controllers;

[ApiController]
[Route("api/v1/ingestion")]
public class OptionChainController : ControllerBase
{
    private readonly IOptionChainRepository _repository;

    public OptionChainController(IOptionChainRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("options-chain/{stockId}/{expiryDate}")]
    public async Task<IActionResult> GetOptionChain(string stockId, string expiryDate)
    {
        var snapshot = await _repository.GetSnapshotAsync(stockId, expiryDate);
        if (snapshot == null)
        {
            return NotFound($"Option chain snapshot not found for {stockId} expiring on {expiryDate}.");
        }
        return Ok(snapshot);
    }
}
