namespace Lyt.Avalonia.AstroPic.Workflow.Collection;

// See if we can create a base class 
public sealed partial class ThumbnailsPanelViewModel : ViewModel<ThumbnailsPanelView>, ISelectListener
{
    private readonly AstroPicModel astroPicModel;
    private readonly CollectionViewModel collectionViewModel;
    private readonly List<Provider> providers;

    [ObservableProperty]
    private bool showMru;

    [ObservableProperty]
    private ObservableCollection<ThumbnailViewModel> thumbnails;

    [ObservableProperty]
    private List<string> providerNames;

    [ObservableProperty]
    private int providersSelectedIndex; 
        
    private PictureMetadata? selectedMetadata;
    private ThumbnailViewModel? selectedThumbnail; 
    private List<ThumbnailViewModel>? allThumbnails;
    private List<ThumbnailViewModel>? filteredThumbnails;

    public ThumbnailsPanelViewModel(CollectionViewModel collectionViewModel)
    {
        this.astroPicModel = App.GetRequiredService<AstroPicModel>();
        this.collectionViewModel = collectionViewModel;
        this.Thumbnails = [];
        this.ProviderNames = [];
        this.providers =
            [.. ( from provider in this.astroPicModel.Providers
              orderby provider.Name
              select provider )];
        this.ShowMru = this.astroPicModel.ShowRecentImages;
        this.PopulateComboBox();
        this.Messenger.Subscribe<LanguageChangedMessage>(this.OnLanguageChanged);
    }

    private void OnLanguageChanged(LanguageChangedMessage _) => this.PopulateComboBox();

    private void PopulateComboBox ()
    {
        var list = new List<string>
        {
            this.Localizer.Lookup ( "Collection.Thumbs.AllServices")
        };

        foreach (Provider provider in this.providers)
        {
            string providerNameLocalized = this.Localizer.Lookup(provider.Name, failSilently: true);
            list.Add(providerNameLocalized);
        }

        this.ProviderNames = list;
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

    public ThumbnailViewModel? SelectedThumbnail => this.selectedThumbnail; 
    public void OnSelect(object selectedObject)
    {
        if (selectedObject is ThumbnailViewModel thumbnailViewModel)
        {
            this.selectedThumbnail = thumbnailViewModel; 
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
            if (this.ProvidersSelectedIndex < 0)
            {
                // Temporarily without a selection : do nothing
                return; 
            }
            else if (this.ProvidersSelectedIndex == 0)
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
                Service.ImageProviderKey key = this.providers[this.ProvidersSelectedIndex - 1].Key;
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
            this.OnSelect(this.Thumbnails[0]);
        }
        else
        {
            this.Thumbnails = [];
            this.selectedMetadata = null;
        }
    }

    partial void OnProvidersSelectedIndexChanged(int value) => this.Filter();

    partial void OnShowMruChanged (bool value)
    {
        this.astroPicModel.ShowRecentImages = value;
        this.Filter();
    }
}
