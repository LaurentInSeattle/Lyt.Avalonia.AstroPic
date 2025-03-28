namespace Lyt.Avalonia.AstroPic.Workflow.Gallery;

public sealed class GalleryViewModel : Bindable<GalleryView>
{
    private readonly AstroPicModel astroPicModel; 

    private bool downloading;
    private bool downloaded ;

    public GalleryViewModel(AstroPicModel astroPicModel)
    {
        this.astroPicModel = astroPicModel;
        this.PictureViewModel = new PictureViewModel(this); 
        this.ThumbnailsPanelViewModel = new ThumbnailsPanelViewModel(this);
    } 

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();
        _ = this.DownloadImages(); 
    }

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);
        _ = this.DownloadImages();
    }

    private async Task DownloadImages ()
    {
        if (this.downloaded || this.downloading)
        {
            return;
        }

        this.downloading = true;
        List<PictureDownload> downloads = await this.astroPicModel.DownloadTodayImages();
        this.downloading = false;
        if ((downloads != null) && (downloads.Count > 0))
        {
            this.downloaded = true;
            Dispatch.OnUiThread(() => { this.LoadImages(downloads); });
        }
    }

    private void LoadImages(List<PictureDownload> downloads) 
        => this.ThumbnailsPanelViewModel.LoadImages(downloads);

    public ThumbnailsPanelViewModel ThumbnailsPanelViewModel 
    { 
        get => this.Get<ThumbnailsPanelViewModel?>() ?? throw new ArgumentNullException("ThumbnailsPanelViewModel"); 
        set => this.Set(value); 
    }

    public PictureViewModel PictureViewModel 
    { 
        get => this.Get<PictureViewModel?>() ?? throw new ArgumentNullException("PictureViewModel");
        set => this.Set(value); 
    }
}
