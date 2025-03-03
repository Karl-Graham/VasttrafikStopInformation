using VasttrafikStopInformation.Core.Models;

namespace VasttrafikStopInformation.Core.Interfaces;

public interface IDepartureService
{

    Task<IEnumerable<Departure>> GetDeparturesAsync(
        string stopId,
        TimeSpan? timeSpan = null,
        int maxDepartures = 10,
        CancellationToken cancellationToken = default);
}