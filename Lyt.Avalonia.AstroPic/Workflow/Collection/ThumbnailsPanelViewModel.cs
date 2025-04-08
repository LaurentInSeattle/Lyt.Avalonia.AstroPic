using Lyt.Avalonia.AstroPic.Service;

namespace Lyt.Avalonia.AstroPic.Workflow.Collection;

// See if we can create a base class 
public sealed class ThumbnailsPanelViewModel : Bindable<ThumbnailsPanelView>, ISelectListener
{
    private readonly CollectionViewModel collectionViewModel;

    private PictureMetadata? selectedMetadata;

    public ThumbnailsPanelViewModel(CollectionViewModel collectionViewModel)
    {
        this.collectionViewModel = collectionViewModel;
        this.Thumbnails = [];
    }

    internal void LoadThumnails(List<Tuple<Picture, byte[]>> thumbnailsCollection)
    {
        this.Thumbnails.Clear();
        List<ThumbnailViewModel> thumbnails = new(thumbnailsCollection.Count);
        foreach (var tuple in thumbnailsCollection)
        {
            thumbnails.Add(
                new ThumbnailViewModel(
                    this, tuple.Item1.PictureMetadata, tuple.Item2, isLarge:false ));
        }

        this.Thumbnails = thumbnails;
        this.selectedMetadata = thumbnails[0].Metadata;
    }

    public void OnSelect(object selectedObject)
    {
        if (selectedObject is ThumbnailViewModel thumbnailViewModel)
        {
            var pictureMetadata = thumbnailViewModel.Metadata;
            if (this.selectedMetadata is null || this.selectedMetadata != pictureMetadata)
            {
                this.selectedMetadata = pictureMetadata;
                this.collectionViewModel.Select(pictureMetadata, thumbnailViewModel.ImageBytes);
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

    public List<ThumbnailViewModel> Thumbnails
    {
        get => this.Get<List<ThumbnailViewModel>?>() ?? throw new ArgumentNullException("ThumbnailsPanelViewModel");
        set => this.Set(value);
    }
}
