using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;
using VasttrafikStopInformation.Core.Interfaces;
using VasttrafikStopInformation.Core.Models;
using VasttrafikStopInformation.Core.Settings;
using VasttrafikStopInformation.Infrastructure.Models;

namespace VasttrafikStopInformation.Infrastructure.Services;

public class VasttrafikDepartureService : IDepartureService
{
    private readonly HttpClient _httpClient;
    private readonly VasttrafikApiSettings _settings;
    private readonly ITokenService _tokenService;
    private readonly ILogger<VasttrafikDepartureService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public VasttrafikDepartureService(
        HttpClient httpClient,
        IOptions<VasttrafikApiSettings> settings,
        ITokenService tokenService,
        ILogger<VasttrafikDepartureService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _httpClient.BaseAddress = new Uri("https://ext-api.vasttrafik.se/");
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        _logger.LogInformation("Base URL: {BaseUrl}", _settings.BaseUrl);
    }

    public async Task<IEnumerable<Departure>> GetDeparturesAsync(
        string stopId,
        TimeSpan? timeSpan = null,
        int maxDepartures = 10,
        CancellationToken cancellationToken = default)
    {
        timeSpan ??= TimeSpan.FromHours(1); // Default to 1 hour ahead
        int timeSpanMinutes = (int)timeSpan.Value.TotalMinutes;

        try
        {
            var accessToken = await _tokenService.GetAccessTokenAsync(cancellationToken);
            _logger.LogInformation("Fetching departures for stop {StopId}", stopId);

            string apiPath = _settings.BaseUrl.TrimEnd('/').Substring(_settings.BaseUrl.IndexOf("/pr"));
            var requestUri = $"{apiPath}/stop-areas/{stopId}/departures?timeSpanInMinutes={timeSpanMinutes}&maxDeparturesPerLineAndDirection=2&limit={maxDepartures}&offset=0&includeOccupancy=false";

            _logger.LogInformation("Request URL: {RequestUrl}", _httpClient.BaseAddress + requestUri.TrimStart('/'));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request, cancellationToken);

            _logger.LogInformation("Response status: {StatusCode}", response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Error response content: {Content}", errorContent);
                response.EnsureSuccessStatusCode();
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogDebug("Response content: {Content}", content);

            var departuresResponse = JsonSerializer.Deserialize<DeparturesResponse>(content, _jsonOptions);

            if (departuresResponse?.Results == null)
            {
                _logger.LogWarning("No departures found or response was invalid");
                return Enumerable.Empty<Departure>();
            }

            var departures = departuresResponse.Results.Select(d => new Departure
            {
                Line = d.ServiceJourney?.Line?.Designation ?? "Unknown",
                Direction = d.ServiceJourney?.Direction ?? "Unknown",
                TransportMode = d.ServiceJourney?.Line?.TransportMode ?? "Unknown",
                ScheduledDepartureTime = d.PlannedTime,
                EstimatedDepartureTime = d.EstimatedTime,
                Platform = d.StopPoint?.Platform
            }).ToList();

            _logger.LogInformation("Successfully retrieved {Count} departures", departures.Count);
            return departures;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching departures for stop {StopId}", stopId);
            throw;
        }
    }
}
