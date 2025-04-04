namespace Lyt.Avalonia.AstroPic.Workflow.Shared;

using static FileManagerModel; 

public sealed class PictureViewModel : Bindable<PictureView>
{
    public const int ThumbnailWidth = 280;

    private readonly AstroPicModel astroPicModel;

    private PictureMetadata? pictureMetadata;
    private byte[]? imageBytes; 

    public PictureViewModel()
    {
        this.astroPicModel = ApplicationBase.GetRequiredService<AstroPicModel>();
        this.Messenger.Subscribe<ZoomRequestMessage>(this.OnZoomRequest);
        this.DisablePropertyChangedLogging = true ;
    }

    internal void Select(PictureMetadata pictureMetadata, byte[] imageBytes)
    {
        this.pictureMetadata = pictureMetadata;
        this.imageBytes = imageBytes;
        var bitmap = WriteableBitmap.Decode(new MemoryStream(imageBytes));
        this.LoadImage(bitmap);
        var metadata = this.pictureMetadata;
        this.Provider = this.astroPicModel.ProviderName(metadata.Provider);
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
            else if (metadata.Description.Length < 400)
            {
                height = 80.0;
            }
            else if (metadata.Description.Length < 800)
            {
                height = 120.0;
            }
            else 
            {
                height = 160.0;
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
        if (this.pictureMetadata is not null)
        {
            Schedule.OnUiThread(
                250, 
                () => { this.Profiler.MemorySnapshot(this.pictureMetadata.Provider.ToString()); }, DispatcherPriority.ApplicationIdle);
        } 
    }

    internal void SetWallpaper() 
    {
        if (this.pictureMetadata is null || this.imageBytes is null)
        {
            return;
        }

        this.astroPicModel.SetWallpaper(this.pictureMetadata, this.imageBytes); 
    }

    internal void AddToCollection()
    {
        if (this.pictureMetadata is null || this.imageBytes is null)
        {
            return;
        }

        var writeableBitmap = 
            WriteableBitmap.DecodeToWidth(new MemoryStream(this.imageBytes), ThumbnailWidth) ; 
        byte[] thumbnailBytes = writeableBitmap.EncodeToJpeg();
        this.astroPicModel.AddToCollection(this.pictureMetadata, this.imageBytes, thumbnailBytes);
    }

    internal void SaveToDesktop()
    {
        if (this.pictureMetadata is null || this.imageBytes is null)
        {
            return;
        }

        try
        {
            var fileManager = ApplicationBase.GetRequiredService<FileManagerModel>();
            fileManager.Save(
                Area.Desktop, Kind.BinaryNoExtension,
                this.pictureMetadata.TodayImageFilePath(),
                this.imageBytes);
        }
        catch (Exception ex)
        {
            string msg = "Failed to save image file: \n" + ex.ToString() ;
            this.Logger.Error(msg);
            var toaster = ApplicationBase.GetRequiredService<IToaster>();
            toaster.Show(
                "File System Error", "Could not save image file",
                10_000, InformationLevel.Warning);
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
