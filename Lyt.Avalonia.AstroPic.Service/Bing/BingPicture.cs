namespace Lyt.Avalonia.AstroPic.Service.Bing;

internal class BingPicture
{
    [JsonPropertyName("copyright")]
    public string? Copyright { get; init; }

    [JsonPropertyName("startdate")]
    public required string Date { get; init; }

    [JsonPropertyName("url")]
    public string? PartialUrl { get; init; }

    [JsonPropertyName("title")]
    public required string Title { get; init; }
}
