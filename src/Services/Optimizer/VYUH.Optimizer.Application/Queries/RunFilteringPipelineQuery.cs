using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VYUH.Optimizer.Application.Interfaces;
using VYUH.Optimizer.Domain;

namespace VYUH.Optimizer.Application.Queries;

public record RunFilteringPipelineQuery(string StockId, string ExpiryDate) : IRequest<FilteringPipelineResult>;

public class RunFilteringPipelineQueryHandler : IRequestHandler<RunFilteringPipelineQuery, FilteringPipelineResult>
{
    private readonly IIngestionServiceClient _client;
    private readonly IFilterThresholdRepository _thresholdRepository;

    public RunFilteringPipelineQueryHandler(IIngestionServiceClient client, IFilterThresholdRepository thresholdRepository)
    {
        _client = client;
        _thresholdRepository = thresholdRepository;
    }

    public async Task<FilteringPipelineResult> Handle(RunFilteringPipelineQuery request, CancellationToken cancellationToken)
    {
        // 1. Fetch data from Ingestion service
        var optionChain = await _client.GetOptionChainAsync(request.StockId, request.ExpiryDate);
        var indicators = await _client.GetTechnicalIndicatorsAsync(request.StockId);

        if (optionChain == null)
        {
            return new FilteringPipelineResult { StockId = request.StockId, IsPassed = false, RejectedByFilter = "IngestionChainOffline", Timestamp = DateTime.UtcNow };
        }

        // Fetch thresholds from repository (with default fallbacks)
        var mwplLimit = (await _thresholdRepository.GetThresholdAsync("mwpl_limit"))?.ThresholdValue ?? 80.0;
        var minPrice = (await _thresholdRepository.GetThresholdAsync("min_price"))?.ThresholdValue ?? 150.0;
        var maxSpread = (await _thresholdRepository.GetThresholdAsync("max_spread_pct"))?.ThresholdValue ?? 2.0;
        var maxAdx = (await _thresholdRepository.GetThresholdAsync("max_adx"))?.ThresholdValue ?? 40.0;

        // --- Filter 1: MWPL ---
        if (optionChain.MwplPct > mwplLimit)
        {
            return new FilteringPipelineResult { StockId = request.StockId, IsPassed = false, RejectedByFilter = "MWPL", Timestamp = DateTime.UtcNow };
        }

        // --- Filter 2: Earnings week buffer ---
        if (DateTime.TryParse(optionChain.EarningsDate, out var earningsDate) &&
            DateTime.TryParse(optionChain.ExpiryDate, out var expiryDate))
        {
            var diffDays = Math.Abs((earningsDate.Date - expiryDate.Date).Days);
            if (diffDays <= 3) // Within expiry week
            {
                return new FilteringPipelineResult { StockId = request.StockId, IsPassed = false, RejectedByFilter = "Earnings", Timestamp = DateTime.UtcNow };
            }
        }

        // --- Filter 3: Price ---
        if (optionChain.SpotPrice < minPrice)
        {
            return new FilteringPipelineResult { StockId = request.StockId, IsPassed = false, RejectedByFilter = "Price", Timestamp = DateTime.UtcNow };
        }

        // --- Filter 4: Spread ---
        var spot = optionChain.SpotPrice;
        var atmContract = optionChain.Contracts
            .OrderBy(c => Math.Abs(c.StrikePrice - spot))
            .FirstOrDefault();
            
        if (atmContract != null)
        {
            var mid = (atmContract.Ask + atmContract.Bid) / 2.0;
            if (mid > 0)
            {
                var spreadPct = ((atmContract.Ask - atmContract.Bid) / mid) * 100.0;
                if (spreadPct > maxSpread)
                {
                    return new FilteringPipelineResult { StockId = request.StockId, IsPassed = false, RejectedByFilter = "Spread", Timestamp = DateTime.UtcNow };
                }
            }
        }

        // --- Filter 5: Hazard (ADX & Supertrend breakout) ---
        if (indicators != null)
        {
            if (indicators.Adx > maxAdx && !string.IsNullOrEmpty(indicators.SupertrendDirection))
            {
                return new FilteringPipelineResult { StockId = request.StockId, IsPassed = false, RejectedByFilter = "Hazard", Timestamp = DateTime.UtcNow };
            }
        }

        return new FilteringPipelineResult
        {
            StockId = request.StockId,
            IsPassed = true,
            RejectedByFilter = string.Empty,
            Timestamp = DateTime.UtcNow
        };
    }
}
