namespace Lyt.Avalonia.AstroPic.Workflow.Gallery;

public class GalleryViewModel : Bindable<GalleryView>
{
    private bool downloading;
    private bool downloaded ;

    public GalleryViewModel()
    {
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
        var result = await AstroPicService.GetPictures(Provider.Bing, DateTime.Now);
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
        var canvas = this.View.Canvas;
        var image = new Image { Stretch = Stretch.Uniform };
        RenderOptions.SetBitmapInterpolationMode(image, BitmapInterpolationMode.MediumQuality);
        canvas.Children.Clear();
        canvas.Children.Add(image);
        var stream = new MemoryStream(imageBytes);
        var bitmap = WriteableBitmap.Decode(stream);
        canvas.Width = bitmap.Size.Width;
        canvas.Height = bitmap.Size.Height;
        image.Source = bitmap;
        this.ZoomFactor = 2.0;
        Schedule.OnUiThread(50, () => { this.ZoomFactor = 1.0; }, DispatcherPriority.ApplicationIdle);        
    }


    private void OnZoomRequest(ZoomRequestMessage message)
        => this.ZoomFactor = message.ZoomFactor;

    public double ZoomFactor { get => this.Get<double>(); set => this.Set(value); }
}
