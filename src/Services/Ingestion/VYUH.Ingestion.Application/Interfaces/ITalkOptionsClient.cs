using System.Threading.Tasks;
using VYUH.Ingestion.Domain;

namespace VYUH.Ingestion.Application.Interfaces;

public interface ITalkOptionsClient
{
    Task<OptionChainSnapshot?> GetOptionGreeksAsync(string stockId, string expiryDate);
}
