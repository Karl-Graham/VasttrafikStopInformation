namespace VasttrafikStopInformation.Core.Settings;


public class VasttrafikApiSettings
{

    public string BaseUrl { get; set; } = "https://ext-api.vasttrafik.se/pr/v4";


    public string TokenUrl { get; set; } = "https://ext-api.vasttrafik.se/token";


    public string AuthenticationKey { get; set; } = string.Empty;

    public string StopId { get; set; } = string.Empty;
}