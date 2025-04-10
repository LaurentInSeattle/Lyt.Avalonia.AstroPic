namespace Lyt.Avalonia.AstroPic.Workflow.Collection;

// See if we can create a base class 
public sealed class ThumbnailsPanelViewModel : Bindable<ThumbnailsPanelView>, ISelectListener
{
    private readonly AstroPicModel astroPicModel;
    private readonly CollectionViewModel collectionViewModel;
    private readonly List<Provider> providers;

    private PictureMetadata? selectedMetadata;
    private List<ThumbnailViewModel>? allThumbnails;
    private List<ThumbnailViewModel>? filteredThumbnails;

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
        this.ProvidersSelectedIndex = 0; // all
    }

    internal void LoadThumnails(List<Tuple<Picture, byte[]>> thumbnailsCollection)
    {
        this.allThumbnails = new(thumbnailsCollection.Count);
        foreach (var tuple in thumbnailsCollection)
        {
            this.allThumbnails.Add(
                new ThumbnailViewModel(
                    this, tuple.Item1.PictureMetadata, tuple.Item2, isLarge: false));
        }

        this.Filter();
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

    private void Filter()
    {
        if ((this.allThumbnails is not null) && (this.allThumbnails.Count > 0))
        {
            if ( this.ProvidersSelectedIndex == 0) 
            {
                if (this.ShowMru)
                {
                    // thumbails are already ordered by date, just take a few 
                    this.filteredThumbnails = [.. this.allThumbnails.Take(8)];
                }
                else
                {
                    // Nothing to do: just copy the source list
                    this.filteredThumbnails = [.. this.allThumbnails];
                }
            } 
            else // this.ProvidersSelectedIndex > 0
            {
                ProviderKey key = this.providers[this.ProvidersSelectedIndex - 1 ].Key;
                var selectedThumbnails =
                    (from thumbnail in this.allThumbnails
                     where thumbnail.Metadata.Provider == key
                     select thumbnail); 
                if (this.ShowMru)
                {
                    // thumbnails are already ordered by date, just take a few 
                    this.filteredThumbnails = [.. selectedThumbnails.Take(8)];
                }
                else
                {
                    this.filteredThumbnails = [.. selectedThumbnails];
                }
            }
        }
        else
        {
            this.filteredThumbnails = null; 
        }

        if (this.filteredThumbnails is not null)
        {
            this.Thumbnails = [.. this.filteredThumbnails];
            this.selectedMetadata = this.filteredThumbnails[0].Metadata;
        }
        else
        {
            this.Thumbnails = [];
            this.selectedMetadata = null;
        }
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
            this.Filter();
        }
    }

    public bool ShowMru
    {
        get => this.Get<bool>();
        set
        {
            this.Set(value);
            this.astroPicModel.ShowRecentImages = value;
            this.Filter();
        }
    }
}
