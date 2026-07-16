using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VYUH.Risk.Domain;

namespace VYUH.Risk.Application.Queries;

public record PositionEvaluationInput(string PositionId, string StockId, double EntryPremium, double CurrentPremium);

public record EvaluatePositionExitsQuery(List<PositionEvaluationInput> Positions) : IRequest<List<PositionExitEvaluation>>;

public class EvaluatePositionExitsQueryHandler : IRequestHandler<EvaluatePositionExitsQuery, List<PositionExitEvaluation>>
{
    private readonly IExitConfigRepository _repository;

    public EvaluatePositionExitsQueryHandler(IExitConfigRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<PositionExitEvaluation>> Handle(EvaluatePositionExitsQuery request, CancellationToken cancellationToken)
    {
        var results = new List<PositionExitEvaluation>();

        foreach (var pos in request.Positions)
        {
            var config = await _repository.GetExitConfigAsync(pos.StockId);
            var slMult = config?.StopLossMultiplier ?? 3.0; // Default 3x
            var targetPct = config?.TargetDecayPct ?? 80.0; // Default 80% decay

            var isTriggered = false;
            var reason = "NONE";

            // 1. Stop-Loss Trigger (premium rises to 3x entry price)
            if (pos.CurrentPremium >= Math.Round(pos.EntryPremium * slMult, 6))
            {
                isTriggered = true;
                reason = "STOP_LOSS";
            }
            // 2. Profit Target Take Trigger (premium decays by 80% -> current premium <= 20% of entry price)
            else if (pos.CurrentPremium <= Math.Round(pos.EntryPremium * (1.0 - (targetPct / 100.0)), 6))
            {
                isTriggered = true;
                reason = "PROFIT_TARGET";
            }

            results.Add(new PositionExitEvaluation
            {
                PositionId = pos.PositionId,
                StockId = pos.StockId,
                EntryPremium = pos.EntryPremium,
                CurrentPremium = pos.CurrentPremium,
                IsExitTriggered = isTriggered,
                ExitReason = reason
            });
        }

        return results;
    }
}
