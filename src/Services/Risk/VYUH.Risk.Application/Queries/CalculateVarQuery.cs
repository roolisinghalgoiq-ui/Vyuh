using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VYUH.Risk.Application.Interfaces;
using VYUH.Risk.Domain;

namespace VYUH.Risk.Application.Queries;

public record PositionInput(
    string PositionId, 
    string StockId, 
    double Quantity, // Negative for short, positive for long
    double SpotPrice, 
    double Delta, 
    double Gamma, 
    double Vega, 
    double Theta, 
    double ImpliedVolatility);

public record CalculateVarQuery(List<PositionInput> Positions) : IRequest<VarAuditLog>;

public class CalculateVarQueryHandler : IRequestHandler<CalculateVarQuery, VarAuditLog>
{
    private readonly IIngestionServiceClient _client;
    private readonly IVarAuditLogRepository _repository;

    public CalculateVarQueryHandler(IIngestionServiceClient client, IVarAuditLogRepository repository)
    {
        _client = client;
        _repository = repository;
    }

    public async Task<VarAuditLog> Handle(CalculateVarQuery request, CancellationToken cancellationToken)
    {
        const int M = 252; // 252-day history
        const double confidencePercentile = 0.01; // 99% VaR is 1st percentile of losses

        // Map returns for all stocks concurrently
        var stockReturns = new Dictionary<string, List<double>>();
        foreach (var pos in request.Positions)
        {
            if (!stockReturns.ContainsKey(pos.StockId))
            {
                var returns = await _client.GetHistoricalLogReturnsAsync(pos.StockId, M);
                stockReturns[pos.StockId] = returns;
            }
        }

        // Compute simulated portfolio price change scenario values
        var pnlScenarios = new double[M];

        for (int j = 0; j < M; j++)
        {
            double scenarioPnl = 0.0;
            foreach (var pos in request.Positions)
            {
                var returns = stockReturns[pos.StockId];
                // Fallback to 0.0 if not enough data
                var r_j = returns.Count > j ? returns[j] : 0.0;

                // 1. Simulated Spot Price change
                var deltaS = pos.SpotPrice * (Math.Exp(r_j) - 1.0);

                // 2. Option price valuation change using Taylor series Greeks approximation
                // (IV shift assumed 0.0, time step 1-day = 1.0/365.0)
                var deltaT = 1.0 / 365.0;
                var deltaOptionPrice = (pos.Delta * deltaS) + (0.5 * pos.Gamma * deltaS * deltaS) + (pos.Theta * deltaT);

                // 3. Qty contribution to portfolio PnL scenario
                scenarioPnl += pos.Quantity * deltaOptionPrice;
            }
            pnlScenarios[j] = scenarioPnl;
        }

        // Sort scenarions in ascending order (losses are negative values)
        var sortedPnl = pnlScenarios.OrderBy(x => x).ToList();

        // 99% VaR is the 1st percentile of sorted distribution losses
        int varIndex = (int)Math.Floor(M * confidencePercentile);
        varIndex = Math.Clamp(varIndex, 0, M - 1);
        double varValue = -sortedPnl[varIndex];

        // Store log audit record
        double portfolioValue = request.Positions.Sum(p => Math.Abs(p.Quantity * p.SpotPrice));
        var log = new VarAuditLog
        {
            LogId = Guid.NewGuid(),
            PortfolioValue = Math.Round(portfolioValue, 2),
            ComputedVar99 = Math.Round(varValue, 2),
            CalculatedAt = DateTime.UtcNow
        };

        await _repository.AddLogAsync(log);

        return log;
    }
}
