using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VYUH.Gateway.Domain;

namespace VYUH.Gateway.Application.Queries;

public record LocalPositionInput(string StockId, string OptionType, double StrikePrice, int Quantity);

public record ReconcilePositionsQuery(List<LocalPositionInput> LocalPositions) : IRequest<ReconciliationSummary>;

public class ReconciliationSummary
{
    public int LocalPositionsCount { get; set; }
    public int BrokerPositionsCount { get; set; }
    public int MismatchesCount { get; set; }
    public List<PositionReconciliationMismatch> Mismatches { get; set; } = new();
}

public class ReconcilePositionsQueryHandler : IRequestHandler<ReconcilePositionsQuery, ReconciliationSummary>
{
    private readonly IBrokerClient _brokerClient;
    private readonly IPositionReconciliationMismatchRepository _repository;

    public ReconcilePositionsQueryHandler(IBrokerClient brokerClient, IPositionReconciliationMismatchRepository repository)
    {
        _brokerClient = brokerClient;
        _repository = repository;
    }

    public async Task<ReconciliationSummary> Handle(ReconcilePositionsQuery request, CancellationToken cancellationToken)
    {
        var brokerPositions = await _brokerClient.GetBrokerActivePositionsAsync();
        var mismatches = new List<PositionReconciliationMismatch>();

        var localPositions = request.LocalPositions;

        // 1. Check local positions against broker
        foreach (var local in localPositions)
        {
            var brokerMatch = brokerPositions.FirstOrDefault(b => 
                b.StockId.Equals(local.StockId, StringComparison.OrdinalIgnoreCase) &&
                b.OptionType.Equals(local.OptionType, StringComparison.OrdinalIgnoreCase) &&
                Math.Abs(b.StrikePrice - local.StrikePrice) < 0.01);

            if (brokerMatch == null)
            {
                // Discrepancy: exists locally but missing at broker
                var mismatch = new PositionReconciliationMismatch
                {
                    MismatchId = Guid.NewGuid(),
                    StockId = local.StockId,
                    OptionType = local.OptionType,
                    StrikePrice = local.StrikePrice,
                    LocalQty = local.Quantity,
                    BrokerQty = 0,
                    DiscrepancyType = "MISSING_BROKER_POSITION",
                    Resolved = false,
                    DetectedAt = DateTime.UtcNow
                };
                mismatches.Add(mismatch);
                await _repository.AddMismatchAsync(mismatch);
            }
            else if (brokerMatch.Quantity != local.Quantity)
            {
                // Discrepancy: quantity mismatch
                var mismatch = new PositionReconciliationMismatch
                {
                    MismatchId = Guid.NewGuid(),
                    StockId = local.StockId,
                    OptionType = local.OptionType,
                    StrikePrice = local.StrikePrice,
                    LocalQty = local.Quantity,
                    BrokerQty = brokerMatch.Quantity,
                    DiscrepancyType = "QUANTITY_MISMATCH",
                    Resolved = false,
                    DetectedAt = DateTime.UtcNow
                };
                mismatches.Add(mismatch);
                await _repository.AddMismatchAsync(mismatch);
            }
        }

        // 2. Check broker positions that don't exist locally
        foreach (var broker in brokerPositions)
        {
            var localMatch = localPositions.FirstOrDefault(l => 
                l.StockId.Equals(broker.StockId, StringComparison.OrdinalIgnoreCase) &&
                l.OptionType.Equals(broker.OptionType, StringComparison.OrdinalIgnoreCase) &&
                Math.Abs(l.StrikePrice - broker.StrikePrice) < 0.01);

            if (localMatch == null)
            {
                // Discrepancy: exists at broker but missing locally
                var mismatch = new PositionReconciliationMismatch
                {
                    MismatchId = Guid.NewGuid(),
                    StockId = broker.StockId,
                    OptionType = broker.OptionType,
                    StrikePrice = broker.StrikePrice,
                    LocalQty = 0,
                    BrokerQty = broker.Quantity,
                    DiscrepancyType = "EXCESS_BROKER_POSITION",
                    Resolved = false,
                    DetectedAt = DateTime.UtcNow
                };
                mismatches.Add(mismatch);
                await _repository.AddMismatchAsync(mismatch);
            }
        }

        return new ReconciliationSummary
        {
            LocalPositionsCount = localPositions.Count,
            BrokerPositionsCount = brokerPositions.Count,
            MismatchesCount = mismatches.Count,
            Mismatches = mismatches
        };
    }
}
