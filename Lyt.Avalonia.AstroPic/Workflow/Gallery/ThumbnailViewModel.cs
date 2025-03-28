namespace Lyt.Avalonia.AstroPic.Workflow.Gallery;

public sealed class ThumbnailViewModel : Bindable<ThumbnailView>
{
    private readonly PictureDownload download;
    private readonly ThumbnailsPanelViewModel parent;

    public ThumbnailViewModel(ThumbnailsPanelViewModel parent, PictureDownload download)
    {
        this.parent = parent;
        this.download = download;
        this.Provider = this.download.PictureMetadata.Provider.ToString();
        var bitmap = WriteableBitmap.DecodeToWidth(new MemoryStream(this.download.ImageBytes), 360);
        this.Thumbnail = bitmap;
    }

    public string Provider { get => this.Get<string>()!; set => this.Set(value); }

    public WriteableBitmap Thumbnail { get => this.Get<WriteableBitmap>()!; set => this.Set(value); }
}
