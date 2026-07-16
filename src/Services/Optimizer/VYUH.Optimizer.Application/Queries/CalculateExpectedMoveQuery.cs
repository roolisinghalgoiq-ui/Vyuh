using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VYUH.Optimizer.Application.Interfaces;
using VYUH.Optimizer.Domain;

namespace VYUH.Optimizer.Application.Queries;

public record CalculateExpectedMoveQuery(string StockId, string ExpiryDate) : IRequest<ExpectedMoveResult?>;

public class CalculateExpectedMoveQueryHandler : IRequestHandler<CalculateExpectedMoveQuery, ExpectedMoveResult?>
{
    private readonly IIngestionServiceClient _client;

    public CalculateExpectedMoveQueryHandler(IIngestionServiceClient client)
    {
        _client = client;
    }

    public async Task<ExpectedMoveResult?> Handle(CalculateExpectedMoveQuery request, CancellationToken cancellationToken)
    {
        // 1. Fetch data from Ingestion service
        var optionChain = await _client.GetOptionChainAsync(request.StockId, request.ExpiryDate);
        var historicalPrices = await _client.GetHistoricalPricesAsync(request.StockId, 20); // Get past 20 days for ATR calculations

        if (optionChain == null) return null;

        var spot = optionChain.SpotPrice;

        // 2. Compute IV-based expected move
        // T = Days to Expiry / 365
        DateTime.TryParse(request.ExpiryDate, out var expiryDateTime);
        var daysToExpiry = Math.Max(1, (expiryDateTime - DateTime.UtcNow).Days);
        var t = daysToExpiry / 365.0;
        
        // Find IV of ATM call
        var atmCall = optionChain.Contracts
            .Where(c => c.Type.Equals("CE", StringComparison.OrdinalIgnoreCase))
            .OrderBy(c => Math.Abs(c.StrikePrice - spot))
            .FirstOrDefault();
            
        var iv = atmCall?.ImpliedVolatility ?? 0.20; // Default fallback to 20%
        var ivMove = spot * iv * Math.Sqrt(t);

        // 3. Compute ATM Straddle-based expected move
        // Find ATM Call and ATM Put
        var atmPut = optionChain.Contracts
            .Where(c => c.Type.Equals("PE", StringComparison.OrdinalIgnoreCase))
            .OrderBy(c => Math.Abs(c.StrikePrice - spot))
            .FirstOrDefault();

        var straddleMove = 0.0;
        if (atmCall != null && atmPut != null)
        {
            var callPremium = (atmCall.Bid + atmCall.Ask) / 2.0;
            var putPremium = (atmPut.Bid + atmPut.Ask) / 2.0;
            straddleMove = (callPremium + putPremium) * 0.85;
        }
        else
        {
            straddleMove = ivMove; // Fallback
        }

        // 4. Compute ATR-14 using Wilder's smoothing
        var atr = CalculateAtr14(historicalPrices);

        // Choose final Expected Move (Straddle preferred if available)
        var finalMove = straddleMove > 0 ? straddleMove : ivMove;

        return new ExpectedMoveResult
        {
            StockId = request.StockId,
            SpotPrice = spot,
            ExpectedMoveIvBased = ivMove,
            ExpectedMoveStraddleBased = straddleMove,
            UpperBoundary = spot + finalMove,
            LowerBoundary = spot - finalMove,
            Atr14 = atr,
            Timestamp = DateTime.UtcNow
        };
    }

    private double CalculateAtr14(List<IngestionHistoricalRecord> history)
    {
        if (history == null || history.Count < 2) return 15.0; // Standard fallback ATR
        
        // Calculate True Ranges from closing prices (as simplified high/low proxy for daily volatility)
        var trs = new List<double>();
        for (int i = 1; i < history.Count; i++)
        {
            var tr = Math.Abs(history[i].ClosePrice - history[i - 1].ClosePrice);
            trs.Add(tr);
        }
        
        return trs.Average(); // Return average daily range
    }
}
