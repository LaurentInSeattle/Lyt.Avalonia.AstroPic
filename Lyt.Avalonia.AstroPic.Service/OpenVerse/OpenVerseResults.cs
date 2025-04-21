namespace Lyt.Avalonia.AstroPic.Service.OpenVerse;

internal class OpenVerseResults
{
    [JsonPropertyName("results")]
    public List<OpenVersePicture> OpenVersePictures { get; init; } = [];
}
