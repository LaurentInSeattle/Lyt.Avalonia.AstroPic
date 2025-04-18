namespace Lyt.Avalonia.AstroPic.Service.Epic; 

internal class EpicPicture
{
    [JsonPropertyName("date")]
    public required string Date { get; init; }

    [JsonPropertyName("caption")]
    public string? Copyright { get; init; }

    [JsonPropertyName("image")]
    public string? PhotoPartialUrl { get; init; }

    [JsonPropertyName("identifier")]
    public string? Title { get; init; }
}
