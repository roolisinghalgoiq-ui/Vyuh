using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using VYUH.Optimizer.Application.Interfaces;
using VYUH.Optimizer.Domain;

namespace VYUH.Optimizer.Application.Commands;

public record QueueProposalCommand(
    string StockId,
    string ParameterName,
    string CurrentValue,
    string ProposedValue,
    decimal ConfidenceScore,
    string Reasoning
) : IRequest<AiParameterProposal>;

public class QueueProposalCommandHandler : IRequestHandler<QueueProposalCommand, AiParameterProposal>
{
    private readonly IAiParameterProposalRepository _repository;

    public QueueProposalCommandHandler(IAiParameterProposalRepository repository)
    {
        _repository = repository;
    }

    public async Task<AiParameterProposal> Handle(QueueProposalCommand request, CancellationToken cancellationToken)
    {
        var proposal = new AiParameterProposal
        {
            ProposalId = Guid.NewGuid(),
            StockId = request.StockId,
            ParameterName = request.ParameterName,
            CurrentValue = request.CurrentValue,
            ProposedValue = request.ProposedValue,
            ConfidenceScore = request.ConfidenceScore,
            Reasoning = request.Reasoning,
            Status = "PENDING",
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddProposalAsync(proposal);
        return proposal;
    }
}
