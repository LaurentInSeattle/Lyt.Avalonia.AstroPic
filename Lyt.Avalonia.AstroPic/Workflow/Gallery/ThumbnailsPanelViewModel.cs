namespace Lyt.Avalonia.AstroPic.Workflow.Gallery;

public sealed class ThumbnailsPanelViewModel : Bindable<ThumbnailsPanelView>
{
    private readonly GalleryViewModel galleryViewModel;

    private PictureDownload? selectedDownload; 

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

    internal void Select(PictureDownload download)
    {
        foreach (ThumbnailViewModel thumbnailViewModel in this.Thumbnails)
        {
            thumbnailViewModel.Deselect(download); 
        }

        if (this.selectedDownload is null || this.selectedDownload != download)
        {
            this.selectedDownload = download; 
            this.galleryViewModel.Select(download);
        } 
    }

    public List<ThumbnailViewModel> Thumbnails
    {
        get => this.Get<List<ThumbnailViewModel>?>() ?? throw new ArgumentNullException("ThumbnailsPanelViewModel");
        set => this.Set(value);
    }
}
