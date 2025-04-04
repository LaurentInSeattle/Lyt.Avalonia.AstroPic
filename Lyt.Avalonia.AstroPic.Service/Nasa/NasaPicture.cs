﻿namespace Lyt.Avalonia.AstroPic.Service.Nasa; 

internal class NasaPicture
{
    [JsonPropertyName("copyright")]
    public string? Copyright { get; init; }

    [JsonPropertyName("date")]
    public required string Date { get; init; }

    [JsonPropertyName("hdurl")]
    public string? HdImageUrl { get; init; }

    [JsonPropertyName("url")]
    public string? MediaUrl { get; init; }

    [JsonPropertyName("media_type")]
    public required string MediaType { get; init; }

    [JsonPropertyName("title")]
    public required string Title { get; init; }

    [JsonPropertyName("explanation")]
    public string? Explanation { get; init ; }
}
