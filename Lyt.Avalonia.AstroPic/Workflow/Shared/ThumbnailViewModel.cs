namespace Lyt.Avalonia.AstroPic.Workflow.Shared;

public sealed class ThumbnailViewModel : Bindable<ThumbnailView>
{
    public const double LargeFontSize = 24.0;
    public const double SmallFontSize = 16.0;

    public const double LargeBorderHeight = 260;
    public const double SmallBorderHeight = 212;

    public const double LargeImageHeight = 200;
    public const double SmallImageHeight = 160;

    public const int LargeThumbnailWidth = 360;
    public const int SmallThumbnailWidth = 240;

    public readonly PictureMetadata Metadata;
    public readonly byte[] ImageBytes;

    private readonly ISelectListener parent;

    /// <summary> 
    /// Creates a thumbnail from a full (large) image - use for downloads 
    /// - OR - 
    /// Creates a thumbnail from a small (thumbnail) image - use for collection 
    /// </summary>
    public ThumbnailViewModel(
        ISelectListener parent, 
        PictureMetadata metadata, byte[] imageBytes, bool isLarge = true )        
    {
        this.DisablePropertyChangedLogging = true;

        this.parent = parent;
        this.Metadata = metadata;
        this.ImageBytes = imageBytes;
        this.BorderHeight = isLarge ? LargeBorderHeight : SmallBorderHeight;
        this.ImageHeight = isLarge ? LargeImageHeight : SmallImageHeight;
        this.FontSize = isLarge ? LargeFontSize : SmallFontSize;
        var model = App.GetRequiredService<AstroPicModel>();
        string providerName = model.ProviderName(this.Metadata.Provider);
        string providerLocalized = this.Localizer.Lookup(providerName, failSilently:true); 
        this.Provider =
            isLarge ?
                providerLocalized :
                string.Concat(providerLocalized, "  ~  " + metadata.Date.ToShortDateString()); 
        var bitmap =
            isLarge  ?
                WriteableBitmap.DecodeToWidth(
                    new MemoryStream(imageBytes), 
                    isLarge ? LargeThumbnailWidth : SmallThumbnailWidth) :
                WriteableBitmap.Decode(new MemoryStream(imageBytes));
        this.Thumbnail = bitmap;
    }

    internal void OnSelect() => this.parent.OnSelect(this);

    internal void ShowDeselected(PictureMetadata metadata)
    {
        if (this.Metadata == metadata)
        {
            return;
        }

        if (this.IsBound)
        {
            this.View.Deselect();
        }
    }

    internal void ShowSelected()
    {
        if (this.IsBound)
        {
            this.View.Select();
        } 
    } 

    public double FontSize { get => this.Get<double>()!; set => this.Set(value); }

    public double BorderHeight { get => this.Get<double>()!; set => this.Set(value); }

    public double ImageHeight { get => this.Get<double>()!; set => this.Set(value); }

    public string Provider { get => this.Get<string>()!; set => this.Set(value); }

    public WriteableBitmap Thumbnail { get => this.Get<WriteableBitmap>()!; set => this.Set(value); }
}
