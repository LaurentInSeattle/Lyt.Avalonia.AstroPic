namespace Lyt.Avalonia.AstroPic.Workflow.Collection;

// See if we can create a base class 
public sealed class ThumbnailsPanelViewModel : Bindable<ThumbnailsPanelView>, ISelectListener
{
    private readonly AstroPicModel astroPicModel;
    private readonly CollectionViewModel collectionViewModel;
    private readonly List<Provider> providers;

    private PictureMetadata? selectedMetadata;

    public ThumbnailsPanelViewModel(CollectionViewModel collectionViewModel)
    {
        this.astroPicModel = App.GetRequiredService<AstroPicModel>();
        this.collectionViewModel = collectionViewModel;
        this.Thumbnails = [];
        this.providers =
            [.. ( from provider in this.astroPicModel.Providers 
              orderby provider.Name 
              select provider )];
        this.ShowMru = this.astroPicModel.ShowRecentImages;
        var list = new List<string>
        {
            "Tutti i servizi."
        };

        foreach (var provider in this.providers)
        {
            list.Add(provider.Name);
        }

        this.Providers = list;
    }

    internal void LoadThumnails(List<Tuple<Picture, byte[]>> thumbnailsCollection)
    {
        List<ThumbnailViewModel> thumbnails = new(thumbnailsCollection.Count);
        foreach (var tuple in thumbnailsCollection)
        {
            thumbnails.Add(
                new ThumbnailViewModel(
                    this, tuple.Item1.PictureMetadata, tuple.Item2, isLarge: false));
        }

        this.Thumbnails = [.. thumbnails];
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

    private void Filter(int providersSelectedIndex, bool showRecent)
    {
        // TODO 
    }

    public ObservableCollection<ThumbnailViewModel> Thumbnails
    {
        get => this.Get<ObservableCollection<ThumbnailViewModel>?>() ?? throw new ArgumentNullException("ThumbnailsPanelViewModel");
        set => this.Set(value);
    }

    public List<string>? Providers { get => this.Get<List<string>?>(); set => this.Set(value); }

    public int ProvidersSelectedIndex 
    { 
        get => this.Get<int>();
        set
        {
            this.Set(value);
            this.Filter(value, this.ShowMru); 
        } 
    }

    public bool ShowMru
    {
        get => this.Get<bool>();
        set
        {
            this.Set(value);
            this.astroPicModel.ShowRecentImages = value;
            this.Filter(this.ProvidersSelectedIndex, value);
        }
    }
}
