namespace Lyt.Avalonia.AstroPic.Workflow.Gallery;

public class GalleryViewModel : Bindable<GalleryView>
{
    private readonly AstroPicModel astroPicModel; 

    private bool downloading;
    private bool downloaded ;

    public GalleryViewModel(AstroPicModel astroPicModel)
    {
        this.astroPicModel = astroPicModel;
    } 

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();

        // Create and bind child views
        // TODO

        _ = this.DownloadImage(); 
    }

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);

        _ = this.DownloadImage();
    }

    private async Task DownloadImage ()
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
    {
        foreach (PictureDownload download in downloads)
        {
            // this.LoadImage(download.ImageBytes);
        } 
    }

}
