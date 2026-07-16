using System.Threading.Tasks;
using VYUH.Ingestion.Domain;

namespace VYUH.Ingestion.Application.Interfaces;

public interface ISuchakClient
{
    Task<TechnicalIndicatorSnapshot> GetTechnicalIndicatorsAsync(string stockId);
}
