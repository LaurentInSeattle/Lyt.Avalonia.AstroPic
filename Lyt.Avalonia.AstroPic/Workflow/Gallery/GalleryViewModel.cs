namespace Lyt.Avalonia.AstroPic.Workflow.Gallery;

public class GalleryViewModel : Bindable<GalleryView>
{
    private bool downloading;
    private bool downloaded ;

    public GalleryViewModel() 
        => this.Messenger.Subscribe<ZoomRequestMessage>(this.OnZoomRequest);

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
        Provider provider = Provider.Bing;
        // Provider provider = Provider.Nasa;
        var result = await AstroPicService.GetPictures(provider, DateTime.Now);
        if ((result != null) && (result.Count > 0))
        {
            var picture = result[0];
            byte[] bytes = await AstroPicService.DownloadPicture(picture);
            this.downloaded = true;
            this.downloading = false;
            Dispatch.OnUiThread(() => { this.LoadImage(bytes); });
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
