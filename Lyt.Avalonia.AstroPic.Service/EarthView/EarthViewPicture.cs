namespace Lyt.Avalonia.AstroPic.Service.EarthView;

internal class EarthViewPicture
{
    [JsonPropertyName("slug")]
    public string? Slug { get; init; }

    [JsonPropertyName("lat")]
    public double Latitude { get; init; }

    [JsonPropertyName("lng")]
    public double Longitude { get; init; }
}
