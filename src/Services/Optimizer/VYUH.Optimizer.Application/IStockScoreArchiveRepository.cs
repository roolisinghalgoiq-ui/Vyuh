using System.Threading.Tasks;
using VYUH.Optimizer.Domain;

namespace VYUH.Optimizer.Application;

public interface IStockScoreArchiveRepository
{
    Task SaveScoreAsync(StockScoreArchive score);
}
