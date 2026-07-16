using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VYUH.Optimizer.Application.Interfaces;
using VYUH.Optimizer.Application.Services;
using VYUH.Optimizer.Domain;

namespace VYUH.Optimizer.Application.Commands;

public record CalculateStockScoresCommand(string StockId, string ExpiryDate) : IRequest<double>;

public class CalculateStockScoresCommandHandler : IRequestHandler<CalculateStockScoresCommand, double>
{
    private readonly IIngestionServiceClient _client;
    private readonly IStockScoreArchiveRepository _archiveRepository;
    private readonly IRedisSortedSetService _redisService;

    public CalculateStockScoresCommandHandler(
        IIngestionServiceClient client, 
        IStockScoreArchiveRepository archiveRepository,
        IRedisSortedSetService redisService)
    {
        _client = client;
        _archiveRepository = archiveRepository;
        _redisService = redisService;
    }

    public async Task<double> Handle(CalculateStockScoresCommand request, CancellationToken cancellationToken)
    {
        // 1. Fetch Option Chain, Indicators, and Pricing from Ingestion
        var optionChain = await _client.GetOptionChainAsync(request.StockId, request.ExpiryDate);
        var indicators = await _client.GetTechnicalIndicatorsAsync(request.StockId);
        
        if (optionChain == null || indicators == null) return 0.0;

        // 2. Parse score parameters
        var spot = optionChain.SpotPrice;
        var ivp = optionChain.IvPercentile;
        
        // ATM Contract for IV and Liquidity
        var atmContract = optionChain.Contracts
            .OrderBy(c => Math.Abs(c.StrikePrice - spot))
            .FirstOrDefault();
            
        var iv = atmContract?.ImpliedVolatility ?? 0.20;
        var lsStock = atmContract?.LiquidityScore ?? 80.0;

        // IV Rank (mock/calculate from history - let's set equal to IVP in simple fallback)
        var ivr = ivp; 

        // Premium Richness
        var historicalPrices = await _client.GetHistoricalPricesAsync(request.StockId, 30);
        var hv30 = CalculateHv30(historicalPrices);
        var pr = hv30 > 0 ? (iv / hv30) : 1.0;
        var prNorm = Math.Min(100.0, 50.0 * pr);

        // Hazard Score based on directional trend strength
        var adx = indicators.Adx;
        var supertrendActive = !string.IsNullOrEmpty(indicators.SupertrendDirection);
        var haz = Math.Min(100.0, (adx > 40.0 ? adx * 1.25 : adx * 0.5) + (supertrendActive ? 25.0 : 0.0));

        // 3. Compute Overall Stock Score (OSS)
        var oss = 0.35 * ivr + 0.25 * ivp + 0.25 * prNorm + 0.15 * lsStock - 0.20 * haz;
        oss = Math.Max(0.0, Math.Min(100.0, oss)); // Clamped 0 to 100

        // 4. Save Daily Score to PostgreSQL
        var archive = new StockScoreArchive
        {
            StockId = request.StockId,
            Timestamp = DateTime.UtcNow,
            SpotPrice = spot,
            IvPercentile = ivp,
            LiquidityScore = lsStock,
            StrategySelected = "STRANGLE",
            ExpectedYield = oss * 0.02,
            RiskScore = haz
        };
        try
        {
            await _archiveRepository.SaveScoreAsync(archive);
        }
        catch
        {
            // Failover
        }

        // 5. Generate ZSET ranking in Redis (overall_rank)
        var zsetKey = "vyuh:scoring:overall_rank";
        await _redisService.AddToSortedSetAsync(zsetKey, request.StockId, oss);

        return oss;
    }

    private double CalculateHv30(List<IngestionHistoricalRecord> history)
    {
        if (history == null || history.Count < 2) return 0.20;
        
        var logReturns = new List<double>();
        for (int i = 1; i < history.Count; i++)
        {
            var ret = Math.Log(history[i].ClosePrice / history[i - 1].ClosePrice);
            logReturns.Add(ret);
        }
        
        var avg = logReturns.Average();
        var variance = logReturns.Select(val => (val - avg) * (val - avg)).Sum() / (logReturns.Count - 1);
        var dailyVol = Math.Sqrt(variance);
        
        return dailyVol * Math.Sqrt(252.0); // Annualized realized volatility
    }
}
