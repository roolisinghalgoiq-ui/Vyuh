using MediatR;
using System.Threading;
using System.Threading.Tasks;
using VYUH.Ingestion.Domain;

namespace VYUH.Ingestion.Application;

public record UpdateOptionChainCommand(OptionChainSnapshot Snapshot) : IRequest<bool>;

public class UpdateOptionChainCommandHandler : IRequestHandler<UpdateOptionChainCommand, bool>
{
    private readonly IOptionChainRepository _repository;

    public UpdateOptionChainCommandHandler(IOptionChainRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(UpdateOptionChainCommand request, CancellationToken cancellationToken)
    {
        await _repository.SaveSnapshotAsync(request.Snapshot);
        return true;
    }
}
