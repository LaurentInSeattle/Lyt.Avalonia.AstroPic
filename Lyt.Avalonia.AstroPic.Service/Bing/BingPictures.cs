namespace Lyt.Avalonia.AstroPic.Service.Bing;

internal class BingPictures
{
    [JsonPropertyName("images")]
    public required List<BingPicture> BingPictureList { get; init; }
}
