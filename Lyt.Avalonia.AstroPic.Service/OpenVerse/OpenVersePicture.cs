namespace Lyt.Avalonia.AstroPic.Service.OpenVerse;

internal class OpenVersePicture
{
    [JsonPropertyName("creator")]
    public string? Creator { get; init; }

    [JsonPropertyName("attribution")]
    public string? Attribution { get; init; }

    [JsonPropertyName("indexed_on")]
    public required string Date { get; init; }

    [JsonPropertyName("url")]
    public string? Url { get; init; }

    [JsonPropertyName("title")]
    public required string Title { get; init; }

    [JsonPropertyName("license")]
    public string? License { get; init; }

    [JsonPropertyName("license_version")]
    public string? LicenseVersion { get; init; }

    [JsonPropertyName("width")]
    public int? Width { get; init; }
}
