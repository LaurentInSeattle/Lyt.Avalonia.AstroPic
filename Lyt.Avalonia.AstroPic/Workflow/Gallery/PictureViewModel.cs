namespace Lyt.Avalonia.AstroPic.Workflow.Gallery;

public sealed class PictureViewModel : Bindable<PictureView>
{
    private readonly GalleryViewModel galleryViewModel;
    private PictureDownload? download;

    public PictureViewModel(GalleryViewModel galleryViewModel)
    {
        this.galleryViewModel = galleryViewModel;
        this.Messenger.Subscribe<ZoomRequestMessage>(this.OnZoomRequest);
    }

    internal void Select(PictureDownload download)
    {
        this.download = download;
        byte[] imageBytes = download.ImageBytes;
        var bitmap = WriteableBitmap.Decode(new MemoryStream(imageBytes));
        this.LoadImage(bitmap);
    }

    private void LoadImage(WriteableBitmap bitmap)
    {
        var image = new Image { Stretch = Stretch.Uniform };
        RenderOptions.SetBitmapInterpolationMode(image, BitmapInterpolationMode.MediumQuality);
        var canvas = this.View.Canvas;
        canvas.Children.Clear();
        canvas.Children.Add(image);
        canvas.Width = bitmap.Size.Width;
        canvas.Height = bitmap.Size.Height;
        image.Source = bitmap;

        // This is where it gets really weird
        // this.View.InvalidateVisual() ; // does not work , even if dispatched 
        // The view box in the zoom control is stuck to zero bounds 
        this.ZoomFactor = 2.0;
        Schedule.OnUiThread(50, () => { this.ZoomFactor = 1.0; }, DispatcherPriority.ApplicationIdle);
        if (this.download is not null)
        {
            Schedule.OnUiThread(
                250, 
                () => { this.Profiler.MemorySnapshot(this.download.PictureMetadata.Provider.ToString()); }, DispatcherPriority.ApplicationIdle);
        } 
    }

    private void OnZoomRequest(ZoomRequestMessage message)
        => this.ZoomFactor = message.ZoomFactor;

    public double ZoomFactor { get => this.Get<double>(); set => this.Set(value); }
}
