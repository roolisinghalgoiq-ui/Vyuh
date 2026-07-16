using MediatR;
using System.Threading;
using System.Threading.Tasks;
using VYUH.Ingestion.Application.Interfaces;
using VYUH.Ingestion.Domain;

namespace VYUH.Ingestion.Application.Queries;

public record GetTechnicalIndicatorsQuery(string StockId) : IRequest<TechnicalIndicatorSnapshot>;

public class GetTechnicalIndicatorsQueryHandler : IRequestHandler<GetTechnicalIndicatorsQuery, TechnicalIndicatorSnapshot>
{
    private readonly ISuchakClient _suchakClient;

    public GetTechnicalIndicatorsQueryHandler(ISuchakClient suchakClient)
    {
        _suchakClient = suchakClient;
    }

    public async Task<TechnicalIndicatorSnapshot> Handle(GetTechnicalIndicatorsQuery request, CancellationToken cancellationToken)
    {
        return await _suchakClient.GetTechnicalIndicatorsAsync(request.StockId);
    }
}
