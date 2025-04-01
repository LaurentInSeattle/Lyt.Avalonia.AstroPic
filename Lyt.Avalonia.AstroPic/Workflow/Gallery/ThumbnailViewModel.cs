namespace Lyt.Avalonia.AstroPic.Workflow.Gallery;

public sealed class ThumbnailViewModel : Bindable<ThumbnailView>
{
    public const int ThumbnailWidth = 360;

    public  readonly PictureDownload Download;

    private readonly ThumbnailsPanelViewModel parent;

    public ThumbnailViewModel(ThumbnailsPanelViewModel parent, PictureDownload download)
    {
        this.parent = parent;
        this.Download = download;
        var model = App.GetRequiredService<AstroPicModel>();
        this.Provider = model.ProviderName(this.Download.PictureMetadata.Provider);
        var bitmap =
            WriteableBitmap.DecodeToWidth(new MemoryStream(this.Download.ImageBytes), ThumbnailWidth);
        this.Thumbnail = bitmap;
    }

    internal void Select()
    {
        Debug.WriteLine("Selected: " + this.Provider);
        this.parent.Select(this.Download);
    }

    internal void Deselect(PictureDownload download)
    {
        if (this.Download == download)
        {
            return;
        }

        Debug.WriteLine("Deselected: " + this.Provider);
        this.View.Deselect();
    }

    public string Provider { get => this.Get<string>()!; set => this.Set(value); }

    public WriteableBitmap Thumbnail { get => this.Get<WriteableBitmap>()!; set => this.Set(value); }
}
