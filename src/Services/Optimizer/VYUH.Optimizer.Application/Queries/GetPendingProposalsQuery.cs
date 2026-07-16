using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VYUH.Optimizer.Application.Interfaces;
using VYUH.Optimizer.Domain;

namespace VYUH.Optimizer.Application.Queries;

public record GetPendingProposalsQuery() : IRequest<List<AiParameterProposal>>;

public class GetPendingProposalsQueryHandler : IRequestHandler<GetPendingProposalsQuery, List<AiParameterProposal>>
{
    private readonly IAiParameterProposalRepository _repository;

    public GetPendingProposalsQueryHandler(IAiParameterProposalRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<AiParameterProposal>> Handle(GetPendingProposalsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetPendingProposalsAsync();
    }
}
