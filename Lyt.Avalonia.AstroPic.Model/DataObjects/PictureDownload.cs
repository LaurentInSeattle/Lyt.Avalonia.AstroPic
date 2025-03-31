namespace Lyt.Avalonia.AstroPic.Model.DataObjects;

public sealed record class PictureDownload(
    PictureMetadata PictureMetadata, 
    byte[] ImageBytes )
{
    public byte[]? ThumbnailBytes { get; set; } 

    public bool IsValid =>
        !string.IsNullOrWhiteSpace(this.PictureMetadata.Url) &&
        this.PictureMetadata.MediaType == MediaType.Image &&
        this.ImageBytes.Length > 256; 
}
