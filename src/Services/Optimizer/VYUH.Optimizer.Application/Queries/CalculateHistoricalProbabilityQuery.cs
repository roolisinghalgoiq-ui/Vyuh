using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VYUH.Optimizer.Application.Interfaces;
using VYUH.Optimizer.Application.Services;
using VYUH.Optimizer.Domain;

namespace VYUH.Optimizer.Application.Queries;

public record CalculateHistoricalProbabilityQuery(string StockId, double StrikePrice, string ExpiryDate) : IRequest<ProbabilityResult?>;

public class CalculateHistoricalProbabilityQueryHandler : IRequestHandler<CalculateHistoricalProbabilityQuery, ProbabilityResult?>
{
    private readonly IIngestionServiceClient _client;
    private readonly BootstrapSimulationEngine _simulationEngine;

    public CalculateHistoricalProbabilityQueryHandler(IIngestionServiceClient client, BootstrapSimulationEngine simulationEngine)
    {
        _client = client;
        _simulationEngine = simulationEngine;
    }

    public async Task<ProbabilityResult?> Handle(CalculateHistoricalProbabilityQuery request, CancellationToken cancellationToken)
    {
        var optionChain = await _client.GetOptionChainAsync(request.StockId, request.ExpiryDate);
        if (optionChain == null) return null;

        var logReturns = await _client.GetHistoricalLogReturnsAsync(request.StockId, 252);

        return _simulationEngine.RunSimulation(
            request.StockId, 
            optionChain.SpotPrice, 
            request.StrikePrice, 
            request.ExpiryDate, 
            logReturns);
    }
}
