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
        var metadata = this.download.PictureMetadata; 
        this.Provider = metadata.Provider.ToString().BeautifyEnumString();
        this.Title = string.IsNullOrWhiteSpace(metadata.Title) ? string.Empty : metadata.Title;
        this.Copyright = string.IsNullOrWhiteSpace(metadata.Copyright) ? string.Empty : metadata.Copyright;
        this.Description = string.IsNullOrWhiteSpace(metadata.Description) ? string.Empty : metadata.Description;
        double height = 0.0; 
        if (!string.IsNullOrWhiteSpace(metadata.Description))
        {
            if (metadata.Description.Length < 150)
            {
                height = 40.0;
            }
            else if (metadata.Description.Length < 300)
            {
                height = 80.0;
            }
            else
            {
                height = 120.0;
            }
        }

        this.DescriptionHeight =new GridLength(height, GridUnitType.Pixel); 
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

    public string Provider { get => this.Get<string>()!; set => this.Set(value); }

    public string Title { get => this.Get<string>()!; set => this.Set(value); }

    public string Copyright { get => this.Get<string>()!; set => this.Set(value); }

    public string Description { get => this.Get<string>()!; set => this.Set(value); }

    public GridLength DescriptionHeight { get => this.Get<GridLength>(); set => this.Set(value); }
}
