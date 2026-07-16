using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VYUH.Optimizer.Application.Interfaces;
using VYUH.Optimizer.Domain;

namespace VYUH.Optimizer.Application.Queries;

public record SelectOptimalStrikesQuery(string StockId, string ExpiryDate) : IRequest<StrikeSelectionResult?>;

public class SelectOptimalStrikesQueryHandler : IRequestHandler<SelectOptimalStrikesQuery, StrikeSelectionResult?>
{
    private readonly IIngestionServiceClient _client;
    private readonly IStrikeSelectionConfigRepository _configRepository;

    public SelectOptimalStrikesQueryHandler(IIngestionServiceClient client, IStrikeSelectionConfigRepository configRepository)
    {
        _client = client;
        _configRepository = configRepository;
    }

    public async Task<StrikeSelectionResult?> Handle(SelectOptimalStrikesQuery request, CancellationToken cancellationToken)
    {
        // 1. Fetch Chain and indicator pivot points
        var optionChain = await _client.GetOptionChainAsync(request.StockId, request.ExpiryDate);
        var indicators = await _client.GetTechnicalIndicatorsAsync(request.StockId);

        if (optionChain == null || indicators == null) return null;

        // Fetch boundaries from repository (with default fallbacks)
        var minDelta = (await _configRepository.GetConfigAsync("min_delta"))?.SettingValue ?? 0.15;
        var maxDelta = (await _configRepository.GetConfigAsync("max_delta"))?.SettingValue ?? 0.18;

        // 2. Select optimal Put Leg (K_P <= S2 and Delta in [0.15, 0.18])
        var putContracts = optionChain.Contracts
            .Where(c => c.Type.Equals("PE", StringComparison.OrdinalIgnoreCase))
            .Where(c => Math.Abs(c.Delta) >= minDelta && Math.Abs(c.Delta) <= maxDelta)
            .ToList();

        var selectedPut = putContracts.FirstOrDefault(c => c.StrikePrice <= indicators.S2);
        if (selectedPut == null && putContracts.Any())
        {
            // Fallback: closest to S2 support line
            selectedPut = putContracts.OrderBy(c => Math.Abs(c.StrikePrice - indicators.S2)).First();
        }

        // 3. Select optimal Call Leg (K_C >= R2 and Delta in [0.15, 0.18])
        var callContracts = optionChain.Contracts
            .Where(c => c.Type.Equals("CE", StringComparison.OrdinalIgnoreCase))
            .Where(c => Math.Abs(c.Delta) >= minDelta && Math.Abs(c.Delta) <= maxDelta)
            .ToList();

        var selectedCall = callContracts.FirstOrDefault(c => c.StrikePrice >= indicators.R2);
        if (selectedCall == null && callContracts.Any())
        {
            // Fallback: closest to R2 resistance line
            selectedCall = callContracts.OrderBy(c => Math.Abs(c.StrikePrice - indicators.R2)).First();
        }

        if (selectedPut == null || selectedCall == null) return null;

        return new StrikeSelectionResult
        {
            StockId = request.StockId,
            ExpiryDate = request.ExpiryDate,
            SelectedCallStrike = selectedCall.StrikePrice,
            SelectedCallDelta = selectedCall.Delta,
            SelectedPutStrike = selectedPut.StrikePrice,
            SelectedPutDelta = selectedPut.Delta,
            SupportS2 = indicators.S2,
            ResistanceR2 = indicators.R2,
            Timestamp = DateTime.UtcNow
        };
    }
}
