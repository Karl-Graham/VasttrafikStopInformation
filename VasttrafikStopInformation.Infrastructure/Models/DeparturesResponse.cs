using System.Text.Json.Serialization;

namespace VasttrafikStopInformation.Infrastructure.Models;
public class DeparturesResponse
{
    [JsonPropertyName("results")]
    public List<DepartureItem>? Results { get; set; }

    public class DepartureItem
    {
        [JsonPropertyName("detailsReference")]
        public string? DetailsReference { get; set; }

        [JsonPropertyName("serviceJourney")]
        public ServiceJourney? ServiceJourney { get; set; }

        [JsonPropertyName("stopPoint")]
        public StopPoint? StopPoint { get; set; }

        [JsonPropertyName("plannedTime")]
        public DateTime PlannedTime { get; set; }

        [JsonPropertyName("estimatedTime")]
        public DateTime? EstimatedTime { get; set; }

        [JsonPropertyName("isCancelled")]
        public bool IsCancelled { get; set; }

        [JsonPropertyName("isPartCancelled")]
        public bool IsPartCancelled { get; set; }
    }

    public class ServiceJourney
    {
        [JsonPropertyName("gid")]
        public string? Gid { get; set; }

        [JsonPropertyName("direction")]
        public string? Direction { get; set; }

        [JsonPropertyName("directionDetails")]
        public DirectionDetails? DirectionDetails { get; set; }

        [JsonPropertyName("line")]
        public Line? Line { get; set; }
    }

    public class DirectionDetails
    {
        [JsonPropertyName("fullDirection")]
        public string? FullDirection { get; set; }

        [JsonPropertyName("shortDirection")]
        public string? ShortDirection { get; set; }

        [JsonPropertyName("via")]
        public string? Via { get; set; }

        [JsonPropertyName("isFrontEntry")]
        public bool? IsFrontEntry { get; set; }
    }

    public class Line
    {
        [JsonPropertyName("gid")]
        public string? Gid { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("shortName")]
        public string? ShortName { get; set; }

        [JsonPropertyName("designation")]
        public string? Designation { get; set; }

        [JsonPropertyName("backgroundColor")]
        public string? BackgroundColor { get; set; }

        [JsonPropertyName("foregroundColor")]
        public string? ForegroundColor { get; set; }

        [JsonPropertyName("borderColor")]
        public string? BorderColor { get; set; }

        [JsonPropertyName("transportMode")]
        public string? TransportMode { get; set; }

        [JsonPropertyName("transportSubMode")]
        public string? TransportSubMode { get; set; }

        [JsonPropertyName("isWheelchairAccessible")]
        public bool IsWheelchairAccessible { get; set; }
    }

    public class StopPoint
    {
        [JsonPropertyName("gid")]
        public string? Gid { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("platform")]
        public string? Platform { get; set; }

        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
    }
}
