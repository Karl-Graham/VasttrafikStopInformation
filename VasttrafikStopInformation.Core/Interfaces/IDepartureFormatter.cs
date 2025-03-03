using VasttrafikStopInformation.Core.Models;

namespace VasttrafikStopInformation.Core.Interfaces;

public interface IDepartureFormatter
{
    void DisplayDepartures(IEnumerable<Departure> departures, string stopName);
}