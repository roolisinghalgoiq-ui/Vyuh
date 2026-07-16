using MediatR;
using System.Threading;
using System.Threading.Tasks;
using VYUH.Ingestion.Application.Interfaces;
using VYUH.Ingestion.Domain;

namespace VYUH.Ingestion.Application.Commands;

public record FetchOptionGreeksCommand(string StockId, string ExpiryDate) : IRequest<OptionChainSnapshot?>;

public class FetchOptionGreeksCommandHandler : IRequestHandler<FetchOptionGreeksCommand, OptionChainSnapshot?>
{
    private readonly ITalkOptionsClient _talkOptionsClient;
    private readonly IOptionChainRepository _repository;

    public FetchOptionGreeksCommandHandler(ITalkOptionsClient talkOptionsClient, IOptionChainRepository repository)
    {
        _talkOptionsClient = talkOptionsClient;
        _repository = repository;
    }

    public async Task<OptionChainSnapshot?> Handle(FetchOptionGreeksCommand request, CancellationToken cancellationToken)
    {
        // 1. Fetch Greeks from TalkOptions API
        var greeksSnapshot = await _talkOptionsClient.GetOptionGreeksAsync(request.StockId, request.ExpiryDate);
        if (greeksSnapshot == null) return null;

        // 2. Query the current active chain from Redis
        var existingChain = await _repository.GetSnapshotAsync(request.StockId, request.ExpiryDate);
        if (existingChain != null)
        {
            // Merge the Greeks data into our active market tick chain
            existingChain.IvPercentile = greeksSnapshot.IvPercentile;
            
            // Match strikes to update Greeks fields
            foreach (var existingContract in existingChain.Contracts)
            {
                var greekContract = greeksSnapshot.Contracts.Find(c => 
                    c.StrikePrice == existingContract.StrikePrice && c.Type == existingContract.Type);
                    
                if (greekContract != null)
                {
                    existingContract.Delta = greekContract.Delta;
                    existingContract.Gamma = greekContract.Gamma;
                    existingContract.Vega = greekContract.Vega;
                    existingContract.Theta = greekContract.Theta;
                    existingContract.ImpliedVolatility = greekContract.ImpliedVolatility;
                }
            }
            
            // Save the merged snapshot back to Redis
            await _repository.SaveSnapshotAsync(existingChain);
            return existingChain;
        }
        else
        {
            // If no active chain exists in Redis yet, save this new one
            await _repository.SaveSnapshotAsync(greeksSnapshot);
            return greeksSnapshot;
        }
    }
}
