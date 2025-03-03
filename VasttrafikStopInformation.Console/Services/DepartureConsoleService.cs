using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VasttrafikStopInformation.Core.Interfaces;
using VasttrafikStopInformation.Core.Settings;

namespace VasttrafikStopInformation.Console.Services;
public class DepartureConsoleService : BackgroundService
{
    private readonly IDepartureService _departureService;
    private readonly IDepartureFormatter _departureFormatter;
    private readonly VasttrafikApiSettings _apiSettings;
    private readonly ILogger<DepartureConsoleService> _logger;
    private readonly IHostApplicationLifetime _appLifetime;

    public DepartureConsoleService(
        IDepartureService departureService,
        IDepartureFormatter departureFormatter,
        IOptions<VasttrafikApiSettings> apiSettings,
        ILogger<DepartureConsoleService> logger,
        IHostApplicationLifetime appLifetime)
    {
        _departureService = departureService ?? throw new ArgumentNullException(nameof(departureService));
        _departureFormatter = departureFormatter ?? throw new ArgumentNullException(nameof(departureFormatter));
        _apiSettings = apiSettings?.Value ?? throw new ArgumentNullException(nameof(apiSettings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _appLifetime = appLifetime ?? throw new ArgumentNullException(nameof(appLifetime));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Starting departure information service");

            // Get the stop ID from settings
            var stopId = _apiSettings.StopId;
            var stopName = "Bellevue";

            if (string.IsNullOrEmpty(stopId))
            {
                _logger.LogError("No stop ID provided in settings.");
                _appLifetime.StopApplication();
                return;
            }

            // Use default timespan and max departures
            var timeSpan = TimeSpan.FromHours(1);
            var maxDepartures = 20;

            _logger.LogInformation("Fetching departures for stop {StopId} ({StopName}) for the next {TimeSpan} minutes, max {MaxDepartures} departures",
                stopId, stopName, timeSpan.TotalMinutes, maxDepartures);

            // Get departures
            var departures = await _departureService.GetDeparturesAsync(
                stopId,
                timeSpan,
                maxDepartures,
                stoppingToken);

            // Display the departures using the formatter - simulate varying times for demo
            _departureFormatter.DisplayDepartures(departures, stopName);

            // Wait for user input
            System.Console.ReadKey();

            // Stop the application
            _appLifetime.StopApplication();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching departure information");
            _appLifetime.StopApplication();
        }
    }
}