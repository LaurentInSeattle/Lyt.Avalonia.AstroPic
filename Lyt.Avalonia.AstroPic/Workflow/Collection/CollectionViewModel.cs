namespace Lyt.Avalonia.AstroPic.Workflow.Collection;

public sealed partial class CollectionViewModel : ViewModel<CollectionView>
{
    private readonly AstroPicModel astroPicModel;

    [ObservableProperty]
    private ThumbnailsPanelViewModel thumbnailsPanelViewModel;

    [ObservableProperty]
    private DropViewModel dropViewModel;

    [ObservableProperty]
    private StatisticsViewModel statisticsViewModel;

    [ObservableProperty]
    private PictureViewModel pictureViewModel;

    private bool loaded;
    private List<Tuple<Picture, byte[]>>? collectionThumbnails;

    public CollectionViewModel(AstroPicModel astroPicModel)
    {
        this.astroPicModel = astroPicModel;
        this.PictureViewModel = new PictureViewModel(this);
        this.DropViewModel = new DropViewModel();
        this.StatisticsViewModel = new StatisticsViewModel(this.astroPicModel);
        this.ThumbnailsPanelViewModel = new ThumbnailsPanelViewModel(this);
        this.Messenger.Subscribe<ToolbarCommandMessage>(this.OnToolbarCommand);
        this.Messenger.Subscribe<ModelLoadedMessage>(this.OnModelLoaded);
        this.Messenger.Subscribe<CollectionChangedMessage>(this.OnCollectionChanged);
    }

    public override void Activate(object? activationParameters) 
    {
        base.Activate(activationParameters);
        if (this.loaded)
        {
            this.UpdateSelection();
        }
    }

    private void OnModelLoaded(ModelLoadedMessage _)
    {
        if (!this.loaded)
        {
            this.loaded = true;
            this.collectionThumbnails = this.astroPicModel.LoadCollectionThumbnails();
            this.ThumbnailsPanelViewModel.LoadThumnails(this.collectionThumbnails);
        }
        else
        {
            this.UpdateSelection();
        }
    }

    private void OnCollectionChanged(CollectionChangedMessage message)
    {
        this.loaded = false;
        this.OnModelLoaded(new());
        this.UpdateSelection();
    }

    private void UpdateSelection()
        =>  Schedule.OnUiThread(
                200,
                () => { this.ThumbnailsPanelViewModel.UpdateSelection(); },
                DispatcherPriority.Background);

    private void OnToolbarCommand(ToolbarCommandMessage message)
    {
        switch (message.Command)
        {
            case ToolbarCommandMessage.ToolbarCommand.CollectionSetWallpaper:
                this.PictureViewModel.SetWallpaper();
                break;

            case ToolbarCommandMessage.ToolbarCommand.RemoveFromCollection:
                this.PictureViewModel.RemoveFromCollection();
                break;

            case ToolbarCommandMessage.ToolbarCommand.CollectionSaveToDesktop:
                this.PictureViewModel.SaveToDesktop();
                break;

            // Ignore all other commands 
            default:
                break;
        }
    }

    internal void Select(PictureMetadata pictureMetadata, byte[] _)
    {
        // We receive the bytes of the thumbnail so we need to load the full image 
        bool showBadPicture = true;
        try
        {
            string? url = pictureMetadata.Url;
            if (!string.IsNullOrEmpty(url) &&
                this.astroPicModel.Pictures.TryGetValue(url, out Picture? maybePicture) &&
                maybePicture is Picture picture)
            {
                var fileManager = App.GetRequiredService<FileManagerModel>();
                var fileId = new FileId(FileManagerModel.Area.User, FileManagerModel.Kind.BinaryNoExtension, picture.ImageFilePath);
                if (fileManager.Exists(fileId))
                {
                    // Consider caching some of the images ? 
                    byte[] imageBytes = fileManager.Load<byte[]>(fileId);
                    if ((imageBytes != null) && (imageBytes.Length > 256))
                    {
                        this.PictureViewModel.Select(pictureMetadata, imageBytes);
                        showBadPicture = false;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }

        if (showBadPicture)
        {
            this.ShowBadPicture();
        }
    }

    internal bool Select(string path, byte[] imageBytes)
    {
        // Here we receive the bytes of the image dropped in the drop zone 
        try
        {
            PictureMetadata pictureMetadata = new ()
            {
                Provider = Service.ProviderKey.Personal,
                Date = DateTime.Now.Date,
                MediaType = MediaType.Image,
                Url = path,
            };

            this.PictureViewModel.Select(pictureMetadata, imageBytes);
            this.PictureViewModel.AddToCollection();
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            this.ShowBadPicture();
        }

        return false;
    }

    private void ShowBadPicture()
    {
        this.PictureViewModel.Title = this.Localizer.Lookup("Collection.BadPicture") ;
        this.Logger.Warning("Collection: Bad picture!"); 
    }
}
