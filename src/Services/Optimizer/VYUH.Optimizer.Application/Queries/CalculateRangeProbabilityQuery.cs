using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VYUH.Optimizer.Application.Interfaces;
using VYUH.Optimizer.Application.Services;
using VYUH.Optimizer.Domain;

namespace VYUH.Optimizer.Application.Queries;

public record CalculateRangeProbabilityQuery(string StockId, double LowerStrikePrice, double UpperStrikePrice, string ExpiryDate) : IRequest<RangeProbabilityResult?>;

public class CalculateRangeProbabilityQueryHandler : IRequestHandler<CalculateRangeProbabilityQuery, RangeProbabilityResult?>
{
    private readonly IIngestionServiceClient _client;

    public CalculateRangeProbabilityQueryHandler(IIngestionServiceClient client)
    {
        _client = client;
    }

    public async Task<RangeProbabilityResult?> Handle(CalculateRangeProbabilityQuery request, CancellationToken cancellationToken)
    {
        // Boundary condition check
        if (request.LowerStrikePrice >= request.UpperStrikePrice)
        {
            return new RangeProbabilityResult
            {
                StockId = request.StockId,
                SpotPrice = 0,
                ExpiryDate = request.ExpiryDate,
                LowerStrikePrice = request.LowerStrikePrice,
                UpperStrikePrice = request.UpperStrikePrice,
                ImpliedVolatility = 0,
                RiskFreeRate = 0,
                ProbInRange = 0.0,
                Timestamp = DateTime.UtcNow
            };
        }

        // 1. Fetch Ingestion Option Chain for active spot and IV
        var optionChain = await _client.GetOptionChainAsync(request.StockId, request.ExpiryDate);
        if (optionChain == null) return null;

        var spot = optionChain.SpotPrice;

        // 2. Fetch ATM implied volatility
        var atmContract = optionChain.Contracts
            .OrderBy(c => Math.Abs(c.StrikePrice - spot))
            .FirstOrDefault();
            
        var iv = atmContract?.ImpliedVolatility ?? 0.20;
        var r = 0.07;

        // Calculate time to expiry T
        DateTime.TryParse(request.ExpiryDate, out var expiryDateTime);
        var daysToExpiry = Math.Max(1, (expiryDateTime - DateTime.UtcNow).Days);
        var t = daysToExpiry / 365.0;

        // 3. Compute Range probability
        // Prob(K1 <= S_T <= K2) = Phi(d2(K1)) - Phi(d2(K2))
        var d2Lower = BsmMath.CalculateD2(spot, request.LowerStrikePrice, r, iv, t);
        var d2Upper = BsmMath.CalculateD2(spot, request.UpperStrikePrice, r, iv, t);

        var probLower = BsmMath.Phi(d2Lower);
        var probUpper = BsmMath.Phi(d2Upper);

        // Clamped inside probability bounds
        var probInRange = Math.Clamp(probLower - probUpper, 0.0, 1.0);

        return new RangeProbabilityResult
        {
            StockId = request.StockId,
            SpotPrice = spot,
            ExpiryDate = request.ExpiryDate,
            LowerStrikePrice = request.LowerStrikePrice,
            UpperStrikePrice = request.UpperStrikePrice,
            ImpliedVolatility = iv,
            RiskFreeRate = r,
            ProbInRange = probInRange,
            Timestamp = DateTime.UtcNow
        };
    }
}
