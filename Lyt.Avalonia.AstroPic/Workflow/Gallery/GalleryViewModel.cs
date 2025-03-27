using System.Net;

namespace Lyt.Avalonia.AstroPic.Workflow.Gallery;

public class GalleryViewModel : Bindable<GalleryView>
{
    private readonly AstroPicModel astroPicModel; 

    private bool downloading;
    private bool downloaded ;

    public GalleryViewModel(AstroPicModel astroPicModel)
    {
        this.astroPicModel = astroPicModel;
        this.Messenger.Subscribe<ZoomRequestMessage>(this.OnZoomRequest);
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
            this.LoadImage(download.ImageBytes);
        } 
    }

    private void LoadImage(byte[] imageBytes)
    {
        var image = new Image { Stretch = Stretch.Uniform };
        RenderOptions.SetBitmapInterpolationMode(image, BitmapInterpolationMode.MediumQuality);
        var canvas = this.View.Canvas;
        canvas.Children.Clear();
        canvas.Children.Add(image);
        var bitmap = WriteableBitmap.Decode(new MemoryStream(imageBytes));
        canvas.Width = bitmap.Size.Width;
        canvas.Height = bitmap.Size.Height;
        image.Source = bitmap;

        // This is where it gets really weird
        // this.View.InvalidateVisual() ; // does not work , even if dispatched 
        // The view box in the zoom control is stuck to zero bounds 
        this.ZoomFactor = 2.0;
        Schedule.OnUiThread(50, () => { this.ZoomFactor = 1.0; }, DispatcherPriority.ApplicationIdle);
    }

    private void OnZoomRequest(ZoomRequestMessage message)
        => this.ZoomFactor = message.ZoomFactor;

    public double ZoomFactor { get => this.Get<double>(); set => this.Set(value); }
}
