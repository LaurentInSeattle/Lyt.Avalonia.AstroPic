namespace Lyt.Avalonia.AstroPic.Workflow.Gallery;

public sealed class ThumbnailsPanelViewModel : Bindable<ThumbnailsPanelView>
{
    private readonly GalleryViewModel galleryViewModel;

    public ThumbnailsPanelViewModel(GalleryViewModel galleryViewModel)
    {
        this.galleryViewModel = galleryViewModel;
        this.Thumbnails = [];    
    }

    internal void LoadImages(List<PictureDownload> downloads)
    {
        this.Thumbnails.Clear();
        List<ThumbnailViewModel> thumbnails = new(downloads.Count);
        foreach (PictureDownload download in downloads)
        {
            thumbnails.Add( new ThumbnailViewModel( this, download ) );
        }

        this.Thumbnails = thumbnails;
    }

    public List<ThumbnailViewModel> Thumbnails
    {
        get => this.Get<List<ThumbnailViewModel>?>() ?? throw new ArgumentNullException("ThumbnailsPanelViewModel");
        set => this.Set(value);
    }
}
