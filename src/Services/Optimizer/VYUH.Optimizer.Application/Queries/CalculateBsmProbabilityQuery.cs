using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VYUH.Optimizer.Application.Interfaces;
using VYUH.Optimizer.Application.Services;
using VYUH.Optimizer.Domain;

namespace VYUH.Optimizer.Application.Queries;

public record CalculateBsmProbabilityQuery(string StockId, double StrikePrice, string ExpiryDate) : IRequest<BsmProbabilityResult?>;

public class CalculateBsmProbabilityQueryHandler : IRequestHandler<CalculateBsmProbabilityQuery, BsmProbabilityResult?>
{
    private readonly IIngestionServiceClient _client;

    public CalculateBsmProbabilityQueryHandler(IIngestionServiceClient client)
    {
        _client = client;
    }

    public async Task<BsmProbabilityResult?> Handle(CalculateBsmProbabilityQuery request, CancellationToken cancellationToken)
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

        // 3. Compute BSM probability
        var d2 = BsmMath.CalculateD2(spot, request.StrikePrice, r, iv, t);
        var probCallItm = BsmMath.Phi(d2);
        var probPutItm = BsmMath.Phi(-d2);

        return new BsmProbabilityResult
        {
            StockId = request.StockId,
            SpotPrice = spot,
            ExpiryDate = request.ExpiryDate,
            StrikePrice = request.StrikePrice,
            ImpliedVolatility = iv,
            RiskFreeRate = r,
            ProbCallItm = probCallItm,
            ProbPutItm = probPutItm,
            Timestamp = DateTime.UtcNow
        };
    }
}
