namespace Lyt.Avalonia.AstroPic.Workflow.Shared;

public sealed class ThumbnailViewModel : Bindable<ThumbnailView>
{
    public const double LargeFontSize = 24.0;
    public const double SmallFontSize = 14.0;

    public const double LargeBorderHeight = 260;
    public const double SmallBorderHeight = 180;

    public const int LargeThumbnailWidth = 360;
    public const int SmallThumbnailWidth = 240;

    public readonly PictureMetadata Metadata;
    public readonly byte[] ImageBytes;

    private readonly ISelectListener parent;

    public ThumbnailViewModel(
        ISelectListener parent, 
        PictureMetadata metadata, byte[] imageBytes, bool isLarge = true )
    {
        this.DisablePropertyChangedLogging = true;

        this.parent = parent;
        this.Metadata = metadata;
        this.ImageBytes = imageBytes;
        this.BorderHeight = isLarge ? LargeBorderHeight : SmallBorderHeight;
        this.FontSize = isLarge ? LargeFontSize : SmallFontSize;
        var model = App.GetRequiredService<AstroPicModel>();
        this.Provider = model.ProviderName(this.Metadata.Provider);
        var bitmap =
            WriteableBitmap.DecodeToWidth(
                new MemoryStream(imageBytes), 
                isLarge ? LargeThumbnailWidth : SmallThumbnailWidth);
        this.Thumbnail = bitmap;
    }

    internal void OnSelect() => this.parent.OnSelect(this);

    internal void ShowDeselected(PictureMetadata metadata)
    {
        if (this.Metadata == metadata)
        {
            return;
        }

        this.View.Deselect();
    }

    internal void ShowSelected() => this.View.Select();

    public double FontSize { get => this.Get<double>()!; set => this.Set(value); }

    public double BorderHeight { get => this.Get<double>()!; set => this.Set(value); }

    public string Provider { get => this.Get<string>()!; set => this.Set(value); }

    public WriteableBitmap Thumbnail { get => this.Get<WriteableBitmap>()!; set => this.Set(value); }
}
