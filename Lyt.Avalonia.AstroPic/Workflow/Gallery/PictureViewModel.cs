namespace Lyt.Avalonia.AstroPic.Workflow.Gallery; 

public sealed class PictureViewModel : Bindable<PictureView>
{
    private readonly GalleryViewModel galleryViewModel;

    public PictureViewModel(GalleryViewModel galleryViewModel)
    {
        this.galleryViewModel = galleryViewModel;
        this.Messenger.Subscribe<ZoomRequestMessage>(this.OnZoomRequest);
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
