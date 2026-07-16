using System.Threading.Tasks;
using VYUH.Ingestion.Domain;

namespace VYUH.Ingestion.Application;

public interface IOptionChainRepository
{
    Task SaveSnapshotAsync(OptionChainSnapshot snapshot);
    Task<OptionChainSnapshot?> GetSnapshotAsync(string stockId, string expiryDate);
}
