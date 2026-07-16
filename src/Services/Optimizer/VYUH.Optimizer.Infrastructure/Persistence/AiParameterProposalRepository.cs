using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VYUH.Optimizer.Application.Interfaces;
using VYUH.Optimizer.Domain;

namespace VYUH.Optimizer.Infrastructure.Persistence;

public class AiParameterProposalRepository : IAiParameterProposalRepository
{
    private readonly OptimizerDbContext _context;

    public AiParameterProposalRepository(OptimizerDbContext context)
    {
        _context = context;
    }

    public async Task AddProposalAsync(AiParameterProposal proposal)
    {
        await _context.AiParameterProposals.AddAsync(proposal);
        await _context.SaveChangesAsync();
    }

    public async Task<AiParameterProposal?> GetByIdAsync(Guid id)
    {
        return await _context.AiParameterProposals.FindAsync(id);
    }

    public async Task<List<AiParameterProposal>> GetPendingProposalsAsync()
    {
        return await _context.AiParameterProposals
            .Where(p => p.Status == "PENDING")
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task UpdateProposalAsync(AiParameterProposal proposal)
    {
        _context.AiParameterProposals.Update(proposal);
        await _context.SaveChangesAsync();
    }
}
