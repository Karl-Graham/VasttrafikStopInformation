namespace VasttrafikStopInformation.Core.Models;

public class Departure
{

    public string Line { get; set; } = string.Empty;

    public string Direction { get; set; } = string.Empty;

    public string TransportMode { get; set; } = string.Empty;

    public DateTime ScheduledDepartureTime { get; set; }

    public DateTime? EstimatedDepartureTime { get; set; }

    public string? Platform { get; set; }
}