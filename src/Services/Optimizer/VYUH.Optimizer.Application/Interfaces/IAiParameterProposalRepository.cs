using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VYUH.Optimizer.Domain;

namespace VYUH.Optimizer.Application.Interfaces;

public interface IAiParameterProposalRepository
{
    Task AddProposalAsync(AiParameterProposal proposal);
    Task<AiParameterProposal?> GetByIdAsync(Guid id);
    Task<List<AiParameterProposal>> GetPendingProposalsAsync();
    Task UpdateProposalAsync(AiParameterProposal proposal);
}
