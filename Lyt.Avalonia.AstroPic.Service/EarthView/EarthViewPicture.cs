namespace Lyt.Avalonia.AstroPic.Service.EarthView; 

internal class EarthViewPicture
{
    [JsonPropertyName("attribution")]
    public string? Copyright { get; init; }

    [JsonPropertyName("photoUrl")]
    public string? PhotoUrl { get; init; }

    [JsonPropertyName("title")]
    public required string Title { get; init; }
}
