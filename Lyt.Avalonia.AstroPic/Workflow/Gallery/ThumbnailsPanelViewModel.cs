namespace Lyt.Avalonia.AstroPic.Workflow.Gallery;

public sealed partial class ThumbnailsPanelViewModel : ViewModel<ThumbnailsPanelView> , ISelectListener
{
    private readonly GalleryViewModel galleryViewModel;

    [ObservableProperty]
    private List<ThumbnailViewModel> thumbnails;

    private PictureMetadata? selectedMetadata;

    public ThumbnailsPanelViewModel(GalleryViewModel galleryViewModel)
    {
        this.galleryViewModel = galleryViewModel;
        this.Thumbnails = [];
    }

    internal void LoadImages(List<PictureDownload> downloads)
    {
        this.Thumbnails.Clear();
        List<ThumbnailViewModel> thumbnails = new(downloads.Count);
        foreach (PictureDownload download in downloads)
        {
            thumbnails.Add(
                new ThumbnailViewModel(this, download.PictureMetadata, download.ImageBytes));
        }

        this.Thumbnails = thumbnails;

        // Delay a bit so that the UI has time to populate
        Schedule.OnUiThread(
            20 + 50 * this.Thumbnails.Count,
            () =>
            {
                if (this.Thumbnails.Count > 0)
                {
                    this.Thumbnails[0].ShowSelected();
                    this.OnSelect(this.Thumbnails[0]);
                }
            }, DispatcherPriority.Background);
    }

    public void OnSelect(object selectedObject)
    {
        if (selectedObject is ThumbnailViewModel thumbnailViewModel)
        {
            var pictureMetadata = thumbnailViewModel.Metadata; 
            if (this.selectedMetadata is null || this.selectedMetadata != pictureMetadata)
            {
                this.selectedMetadata = pictureMetadata;
                this.galleryViewModel.Select(pictureMetadata, thumbnailViewModel.ImageBytes);
            }

            this.UpdateSelection();
        }
    }

    internal void UpdateSelection()
    {
        if (this.selectedMetadata is not null)
        {
            foreach (ThumbnailViewModel thumbnailViewModel in this.Thumbnails)
            {
                if (thumbnailViewModel.Metadata == this.selectedMetadata)
                {
                    thumbnailViewModel.ShowSelected();
                }
                else
                {
                    thumbnailViewModel.ShowDeselected(this.selectedMetadata);
                }
            }
        }
    }
}
