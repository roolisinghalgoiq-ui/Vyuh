using System.Collections.Generic;
using System.Threading.Tasks;

namespace VYUH.Risk.Application.Interfaces;

public interface IIngestionServiceClient
{
    Task<List<double>> GetHistoricalLogReturnsAsync(string stockId, int periodDays);
}
