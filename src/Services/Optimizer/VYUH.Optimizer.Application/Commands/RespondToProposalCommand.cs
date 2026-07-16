using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using VYUH.Optimizer.Application.Interfaces;
using VYUH.Optimizer.Domain;

namespace VYUH.Optimizer.Application.Commands;

public record RespondToProposalCommand(
    Guid ProposalId,
    bool Approved
) : IRequest<AiParameterProposal?>;

public class RespondToProposalCommandHandler : IRequestHandler<RespondToProposalCommand, AiParameterProposal?>
{
    private readonly IAiParameterProposalRepository _repository;
    private readonly IFilterThresholdRepository _thresholdRepository;
    private readonly IStrikeSelectionConfigRepository _configRepository;

    public RespondToProposalCommandHandler(
        IAiParameterProposalRepository repository,
        IFilterThresholdRepository thresholdRepository,
        IStrikeSelectionConfigRepository configRepository)
    {
        _repository = repository;
        _thresholdRepository = thresholdRepository;
        _configRepository = configRepository;
    }

    public async Task<AiParameterProposal?> Handle(RespondToProposalCommand request, CancellationToken cancellationToken)
    {
        var proposal = await _repository.GetByIdAsync(request.ProposalId);
        if (proposal == null || proposal.Status != "PENDING")
        {
            return proposal;
        }

        proposal.Status = request.Approved ? "APPROVED" : "REJECTED";
        proposal.RespondedAt = DateTime.UtcNow;

        if (request.Approved)
        {
            if (double.TryParse(proposal.ProposedValue, out var newValue))
            {
                var threshold = await _thresholdRepository.GetThresholdAsync(proposal.ParameterName);
                if (threshold != null)
                {
                    threshold.ThresholdValue = newValue;
                    await _thresholdRepository.UpdateThresholdAsync(threshold);
                }
                else
                {
                    var config = await _configRepository.GetConfigAsync(proposal.ParameterName);
                    if (config != null)
                    {
                        config.SettingValue = newValue;
                        await _configRepository.UpdateConfigAsync(config);
                    }
                }
            }
        }

        await _repository.UpdateProposalAsync(proposal);
        return proposal;
    }
}
