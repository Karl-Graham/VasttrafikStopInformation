using Microsoft.Extensions.Logging;
using VasttrafikStopInformation.Core.Interfaces;
using VasttrafikStopInformation.Core.Models;

namespace VasttrafikStopInformation.Console.Services;

public class ConsoleDepartureFormatter : IDepartureFormatter
{
    private readonly ILogger<ConsoleDepartureFormatter> _logger;

    public ConsoleDepartureFormatter(ILogger<ConsoleDepartureFormatter> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void DisplayDepartures(IEnumerable<Departure> departures, string stopName)
    {
        var departuresList = departures.ToList();

        if (!departuresList.Any())
        {
            System.Console.WriteLine($"No departures found for {stopName}.");
            return;
        }

        var sortedDepartures = departuresList.OrderBy(d =>
            d.EstimatedDepartureTime ?? d.ScheduledDepartureTime).ToList();

        System.Console.WriteLine($"\nDepartures from {stopName}:");
        System.Console.WriteLine("=======================\n");

        var now = DateTime.Now;

        foreach (var departure in sortedDepartures)
        {
            CalculateTimeUntilDeparture(now, departure, out int minutesUntil, out string timeUntil);

            var estimatedTime = departure.EstimatedDepartureTime.HasValue &&
                               departure.EstimatedDepartureTime.Value != departure.ScheduledDepartureTime
                ? $" (Est: {departure.EstimatedDepartureTime.Value:HH:mm})"
                : string.Empty;

            var platform = !string.IsNullOrEmpty(departure.Platform)
                ? $" | Platform: {departure.Platform}"
                : string.Empty;

            // Color code departures
            if (minutesUntil <= 5)
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
            }
            else if (minutesUntil <= 15)
            {
                System.Console.ForegroundColor = ConsoleColor.Yellow;
            }
            else
            {
                System.Console.ForegroundColor = ConsoleColor.Green;
            }

            System.Console.WriteLine($"{departure.Line,-4} - {departure.Direction,-30} | {departure.TransportMode,-5} | {departure.ScheduledDepartureTime:HH:mm}{estimatedTime,-15} | {timeUntil,-10}{platform}");
            System.Console.ResetColor();
        }

        System.Console.WriteLine("\nPress any key to exit...");
    }

    private static void CalculateTimeUntilDeparture(DateTime now, Departure departure, out int minutesUntil, out string timeUntil)
    {
        var departureTime = departure.EstimatedDepartureTime ?? departure.ScheduledDepartureTime;
        minutesUntil = Math.Max(0, (int)(departureTime - now).TotalMinutes);
        timeUntil = minutesUntil == 0 ? "Now" : $"in {minutesUntil} min";
    }
}
