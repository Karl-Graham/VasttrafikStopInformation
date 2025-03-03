using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using VasttrafikStopInformation.Core.Interfaces;
using VasttrafikStopInformation.Core.Settings;
using VasttrafikStopInformation.Infrastructure.Models;

namespace VasttrafikStopInformation.Infrastructure.Services;

/// <summary>
/// Service for managing OAuth access tokens for the Västtrafik API
/// </summary>
public class VasttrafikTokenService : ITokenService
{
    private readonly VasttrafikApiSettings _settings;
    private readonly ILogger<VasttrafikTokenService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    private string _accessToken = string.Empty;
    private DateTime _tokenExpiryTime = DateTime.MinValue;

    public VasttrafikTokenService(
        IOptions<VasttrafikApiSettings> settings,
        ILogger<VasttrafikTokenService> logger)
    {
        _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    /// <inheritdoc />
    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        // If we have a valid token, return it
        if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiryTime)
        {
            _logger.LogDebug("Using existing access token");
            return _accessToken;
        }

        _logger.LogInformation("Requesting new access token");

        try
        {
            // Create a new HTTP client for the token request
            using var tokenClient = new HttpClient();

            // Create the request message
            var request = new HttpRequestMessage(HttpMethod.Post, _settings.TokenUrl);

            // Add the required headers
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", _settings.AuthenticationKey);

            // Add the form content
            request.Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");

            // Make the request
            var response = await tokenClient.SendAsync(request, cancellationToken);

            // Log response status
            _logger.LogInformation("Token request status: {StatusCode}", response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Error response content: {Content}", errorContent);
                response.EnsureSuccessStatusCode();
            }

            // Parse the response
            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(content, _jsonOptions);

            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
            {
                throw new InvalidOperationException("Received invalid token response");
            }

            // Save the token and calculate expiry time (with a small safety margin)
            _accessToken = tokenResponse.AccessToken;
            _tokenExpiryTime = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn - 30);

            _logger.LogInformation("Successfully obtained new access token, valid until {ExpiryTime}", _tokenExpiryTime);

            return _accessToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obtaining access token");
            throw;
        }
    }
}