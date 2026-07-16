using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VYUH.Ingestion.Application.Interfaces;
using VYUH.Ingestion.Domain;

namespace VYUH.Ingestion.Application.Queries;

public record GetHistoricalReturnsQuery(string StockId, int PeriodDays) : IRequest<List<HistoricalReturn>>;

public class GetHistoricalReturnsQueryHandler : IRequestHandler<GetHistoricalReturnsQuery, List<HistoricalReturn>>
{
    private readonly IGaneshClient _ganeshClient;

    public GetHistoricalReturnsQueryHandler(IGaneshClient ganeshClient)
    {
        _ganeshClient = ganeshClient;
    }

    public async Task<List<HistoricalReturn>> Handle(GetHistoricalReturnsQuery request, CancellationToken cancellationToken)
    {
        return await _ganeshClient.GetHistoricalReturnsAsync(request.StockId, request.PeriodDays);
    }
}
