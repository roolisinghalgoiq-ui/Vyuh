using System.Collections.Generic;
using System.Threading.Tasks;
using VYUH.Ingestion.Domain;

namespace VYUH.Ingestion.Application.Interfaces;

public interface IGaneshClient
{
    Task<List<HistoricalReturn>> GetHistoricalReturnsAsync(string stockId, int periodDays);
}
