# Västtrafik Stop Information

A .NET 9 console application that fetches and displays live departure information from the Västtrafik API for the "Bellevue" stop in Gothenburg.

## Features

- Authenticates with the Västtrafik API using OAuth 2.0
- Fetches real-time departure information from Västtrafik API
- Displays upcoming departures with scheduled and estimated times
- Shows line numbers, directions, transport modes, and platforms
- Color-codes departures based on time until arrival
- Sorts departures by time
- Shows relative time until departure (in minutes)
- Uses dependency injection and follows SOLID principles
- Implements comprehensive logging for debugging and monitoring

## Project Structure

The solution follows a clean architecture approach with three projects:

- **VasttrafikStopInformation.Core**: Contains domain models, interfaces, and business logic
- **VasttrafikStopInformation.Infrastructure**: Contains API communication and external service implementations
- **VasttrafikStopInformation.Console**: Console application that displays the information

## Architectural Features

The application demonstrates several best practices in software architecture:

### Separation of Concerns
- **Token Service**: Dedicated service for managing OAuth authentication
- **Departure Service**: Handles API communication for retrieving departures
- **Departure Formatter**: Formats departure data for display
- **Console Service**: Orchestrates the application workflow

### Configuration and Validation
- Strong-typed settings with validation
- Fail-fast approach for missing configuration
- Centralized error handling

### Dependency Injection
- All services are registered and resolved via DI
- Services depend on abstractions, not concrete implementations
- Easy to replace components or add new ones

## Configuration

Before running the application, you need to configure your Västtrafik API authentication details in the `appsettings.json` file:

```json
{
  "VasttrafikApi": {
    "BaseUrl": "https://ext-api.vasttrafik.se/pr/v4",
    "TokenUrl": "https://ext-api.vasttrafik.se/token",
    "AuthenticationKey": "your-base64-encoded-authentication-key",
    "BellevueStopId": "9021014001310000"
  }
}
```

The `AuthenticationKey` is a Base64 encoded string of your client ID and client secret in the format `clientId:clientSecret`.

## OAuth 2.0 Authentication

The application implements the OAuth 2.0 Client Credentials flow via a dedicated token service:

1. `ITokenService` provides an abstraction for token management
2. `VasttrafikTokenService` handles token acquisition and caching
3. The service automatically manages token lifecycle, requesting new tokens only when needed
4. Other services can request a valid token without managing OAuth details

## Simulated Departure Times

For demonstration purposes, the application simulates varying departure times since the API in demo mode returns all departures with the same timestamp. In a production environment with a real API key, this simulation would be disabled, and real-time data would be displayed.

## How to Run

1. Make sure you have .NET 9 SDK installed
2. Clone the repository
3. Update the `appsettings.json` with your Västtrafik API authentication key
4. Run the application:

```
dotnet run --project VasttrafikStopInformation.Console
```

## Dependencies

- .NET 9
- Microsoft.Extensions.DependencyInjection
- Microsoft.Extensions.Hosting
- Microsoft.Extensions.Configuration
- Microsoft.Extensions.Logging
- Microsoft.Extensions.Http
- System.Text.Json (Note: Currently using version with known vulnerabilities - update in production) 