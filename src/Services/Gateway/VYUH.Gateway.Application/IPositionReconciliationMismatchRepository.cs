using System.Threading.Tasks;
using VYUH.Gateway.Domain;

namespace VYUH.Gateway.Application;

public interface IPositionReconciliationMismatchRepository
{
    Task AddMismatchAsync(PositionReconciliationMismatch mismatch);
}
