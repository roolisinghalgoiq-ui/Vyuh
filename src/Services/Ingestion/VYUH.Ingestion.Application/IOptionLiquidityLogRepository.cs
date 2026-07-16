using System.Collections.Generic;
using System.Threading.Tasks;
using VYUH.Ingestion.Domain;

namespace VYUH.Ingestion.Application;

public interface IOptionLiquidityLogRepository
{
    Task SaveLogsAsync(List<OptionLiquidityLog> logs);
}
