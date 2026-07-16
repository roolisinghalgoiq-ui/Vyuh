using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VYUH.Optimizer.Application.Interfaces;
using VYUH.Optimizer.Domain;

namespace VYUH.Optimizer.Application.Queries;

public record CalculateStrategySelectionQuery(string StockId, string ExpiryDate) : IRequest<StrategySelectionResult?>;

public class CalculateStrategySelectionQueryHandler : IRequestHandler<CalculateStrategySelectionQuery, StrategySelectionResult?>
{
    private readonly IIngestionServiceClient _client;

    public CalculateStrategySelectionQueryHandler(IIngestionServiceClient client)
    {
        _client = client;
    }

    public async Task<StrategySelectionResult?> Handle(CalculateStrategySelectionQuery request, CancellationToken cancellationToken)
    {
        // 1. Fetch Option Chain and indicators
        var optionChain = await _client.GetOptionChainAsync(request.StockId, request.ExpiryDate);
        var indicators = await _client.GetTechnicalIndicatorsAsync(request.StockId);

        if (optionChain == null || indicators == null) return null;

        var spot = optionChain.SpotPrice;
        var ivp = optionChain.IvPercentile;
        var ivr = ivp; // Simplified rank fallback
        var adx = indicators.Adx;
        var supertrendDir = indicators.SupertrendDirection;
        var trendActive = !string.IsNullOrEmpty(supertrendDir);

        var strategy = "NONE";
        var allocation = 0.0;

        // 2. Strategy Matrix Evaluation
        if (ivr >= 70.0 && adx < 20.0)
        {
            strategy = "STRADDLE"; // Short Straddle
            allocation = 0.15; // 15% capital allocation
        }
        else if (ivr >= 50.0 && adx < 25.0)
        {
            strategy = "STRANGLE"; // Short Strangle
            allocation = 0.10; // 10% capital allocation
        }
        else if (ivr >= 40.0 && adx >= 25.0 && adx <= 35.0)
        {
            strategy = "IRON_CONDOR"; // Defined risk Strangle
            allocation = 0.08; // 8% capital allocation
        }
        else if (ivr < 15.0 && adx > 35.0 && trendActive)
        {
            strategy = "LONG_STRADDLE"; // Volatility breakout purchase
            allocation = 0.05; // 5% capital allocation
        }

        return new StrategySelectionResult
        {
            StockId = request.StockId,
            SpotPrice = spot,
            ExpiryDate = request.ExpiryDate,
            IvPercentile = ivp,
            Adx = adx,
            StrategySelected = strategy,
            AllocationPercentage = allocation,
            Timestamp = DateTime.UtcNow
        };
    }
}
