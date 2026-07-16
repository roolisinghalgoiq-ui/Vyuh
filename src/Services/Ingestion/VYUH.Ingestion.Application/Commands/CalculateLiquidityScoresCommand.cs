using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VYUH.Ingestion.Domain;

namespace VYUH.Ingestion.Application.Commands;

public record CalculateLiquidityScoresCommand(string StockId, string ExpiryDate) : IRequest<List<OptionContract>>;

public class CalculateLiquidityScoresCommandHandler : IRequestHandler<CalculateLiquidityScoresCommand, List<OptionContract>>
{
    private readonly IOptionChainRepository _repository;
    private readonly IOptionLiquidityLogRepository _logRepository;

    public CalculateLiquidityScoresCommandHandler(
        IOptionChainRepository repository, 
        IOptionLiquidityLogRepository logRepository)
    {
        _repository = repository;
        _logRepository = logRepository;
    }

    public async Task<List<OptionContract>> Handle(CalculateLiquidityScoresCommand request, CancellationToken cancellationToken)
    {
        // 1. Fetch Option Chain from Redis
        var snapshot = await _repository.GetSnapshotAsync(request.StockId, request.ExpiryDate);
        if (snapshot == null) return new List<OptionContract>();

        var dailyLogs = new List<OptionLiquidityLog>();

        // 2. Score each contract in the chain
        foreach (var contract in snapshot.Contracts)
        {
            var ls = CalculateLiquidityScore(contract);
            contract.LiquidityScore = ls;

            // Prepare log entity
            dailyLogs.Add(new OptionLiquidityLog
            {
                StockId = request.StockId,
                StrikePrice = contract.StrikePrice,
                OptionType = contract.Type.ToString(),
                ExpiryDate = request.ExpiryDate,
                LiquidityScore = ls,
                Timestamp = DateTime.UtcNow
            });
        }

        // 3. Save scored chain back to Redis
        await _repository.SaveSnapshotAsync(snapshot);

        // 4. Log to PostgreSQL
        try
        {
            await _logRepository.SaveLogsAsync(dailyLogs);
        }
        catch
        {
            // Failover
        }

        // 5. Return contracts where LS >= 40 (Acceptance Criteria)
        return snapshot.Contracts.Where(c => c.LiquidityScore >= 40.0).ToList();
    }

    private double CalculateLiquidityScore(OptionContract contract)
    {
        var midPrice = (contract.Ask + contract.Bid) / 2.0;
        
        var sSpread = 0.0;
        if (midPrice > 0)
        {
            var spreadPct = (contract.Ask - contract.Bid) / midPrice;
            sSpread = Math.Max(0.0, 100.0 * (1.0 - (spreadPct / 0.01)));
        }

        var sOi = Math.Min(100.0, 100.0 * (contract.OpenInterest / 50000.0));
        var sVol = Math.Min(100.0, 100.0 * (contract.Volume / 10000.0));

        return 0.40 * sSpread + 0.30 * sOi + 0.30 * sVol;
    }
}
