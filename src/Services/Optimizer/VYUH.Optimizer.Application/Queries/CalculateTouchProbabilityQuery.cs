using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VYUH.Optimizer.Application.Interfaces;
using VYUH.Optimizer.Application.Services;
using VYUH.Optimizer.Domain;

namespace VYUH.Optimizer.Application.Queries;

public record CalculateTouchProbabilityQuery(string StockId, double StrikePrice, string ExpiryDate) : IRequest<TouchProbabilityResult?>;

public class CalculateTouchProbabilityQueryHandler : IRequestHandler<CalculateTouchProbabilityQuery, TouchProbabilityResult?>
{
    private readonly IIngestionServiceClient _client;

    public CalculateTouchProbabilityQueryHandler(IIngestionServiceClient client)
    {
        _client = client;
    }

    public async Task<TouchProbabilityResult?> Handle(CalculateTouchProbabilityQuery request, CancellationToken cancellationToken)
    {
        // 1. Fetch Ingestion Option Chain for active spot and IV
        var optionChain = await _client.GetOptionChainAsync(request.StockId, request.ExpiryDate);
        if (optionChain == null) return null;

        var spot = optionChain.SpotPrice;

        // 2. Fetch ATM implied volatility
        var atmContract = optionChain.Contracts
            .OrderBy(c => Math.Abs(c.StrikePrice - spot))
            .FirstOrDefault();
            
        var iv = atmContract?.ImpliedVolatility ?? 0.20; // Default fallback to 20%
        var r = 0.07; // Standard 7% risk-free rate for Indian markets

        // Calculate time to expiry T
        DateTime.TryParse(request.ExpiryDate, out var expiryDateTime);
        var daysToExpiry = Math.Max(1, (expiryDateTime - DateTime.UtcNow).Days);
        var t = daysToExpiry / 365.0;

        // 3. Compute BSM Touch probability
        var probTouch = BsmMath.CalculateTouchProbability(spot, request.StrikePrice, r, iv, t);

        return new TouchProbabilityResult
        {
            StockId = request.StockId,
            SpotPrice = spot,
            ExpiryDate = request.ExpiryDate,
            StrikePrice = request.StrikePrice,
            ImpliedVolatility = iv,
            RiskFreeRate = r,
            ProbTouch = probTouch,
            Timestamp = DateTime.UtcNow
        };
    }
}
