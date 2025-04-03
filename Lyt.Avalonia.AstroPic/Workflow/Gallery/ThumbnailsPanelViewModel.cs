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

        // Delay a bit so that the UI has time to populate
        Schedule.OnUiThread(
            20 + 50 * this.Thumbnails.Count, 
            ()=> 
            {
                if (this.Thumbnails.Count > 0)
                {
                    this.Thumbnails[0].ShowSelected();
                    this.OnSelect(this.Thumbnails[0].Download);
                } 
            }, DispatcherPriority.Background);
    }

    internal void OnSelect(PictureDownload download)
    {
        foreach (ThumbnailViewModel thumbnailViewModel in this.Thumbnails)
        {
            thumbnailViewModel.ShowDeselected(download); 
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
