
namespace Lyt.Avalonia.AstroPic.Workflow.Gallery;

using System.Xml.Linq;
using static FileManagerModel; 

public sealed class PictureViewModel : Bindable<PictureView>
{
    public const int ThumbnailWidth = 280;

    private readonly AstroPicModel astroPicModel;
    private readonly GalleryViewModel galleryViewModel;

    private PictureDownload? download;

    public PictureViewModel(GalleryViewModel galleryViewModel)
    {
        this.galleryViewModel = galleryViewModel;
        this.astroPicModel = App.GetRequiredService<AstroPicModel>();
        this.Messenger.Subscribe<ZoomRequestMessage>(this.OnZoomRequest);
    }

    internal void Select(PictureDownload download)
    {
        this.download = download;
        byte[] imageBytes = download.ImageBytes;
        var bitmap = WriteableBitmap.Decode(new MemoryStream(imageBytes));
        this.LoadImage(bitmap);
        var metadata = this.download.PictureMetadata;
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
        if (this.download is not null)
        {
            Schedule.OnUiThread(
                250, 
                () => { this.Profiler.MemorySnapshot(this.download.PictureMetadata.Provider.ToString()); }, DispatcherPriority.ApplicationIdle);
        } 
    }

    internal void SetWallpaper() 
    {
        if( this.download is null)
        {
            return;
        }

        this.astroPicModel.SetWallpaper(this.download); 
    }

    internal void AddToCollection()
    {
        if (this.download is null)
        {
            return;
        }

        var writeableBitmap = 
            WriteableBitmap.DecodeToWidth(new MemoryStream(this.download.ImageBytes), ThumbnailWidth) ; 
        this.download.ThumbnailBytes = writeableBitmap.EncodeToJpeg();
        this.astroPicModel.AddToCollection(this.download);
    }

    internal void SaveToDesktop()
    {
        if (this.download is null)
        {
            return;
        }

        try
        {
            var fileManager = App.GetRequiredService<FileManagerModel>();
            fileManager.Save<byte[]>(
                Area.Desktop, Kind.BinaryNoExtension,
                this.download.PictureMetadata.TodayImageFilePath(),
                this.download.ImageBytes);
        }
        catch (Exception ex)
        {
            string msg = "Failed to save image file: \n" + ex.ToString() ;
            this.Logger.Error(msg);
            var toaster = App.GetRequiredService<IToaster>();
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
