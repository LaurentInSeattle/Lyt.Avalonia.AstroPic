using Lyt.Avalonia.AstroPic.Service;

namespace Lyt.Avalonia.AstroPic.Workflow.Collection;

public sealed class CollectionViewModel : Bindable<CollectionView>
{
    private readonly AstroPicModel astroPicModel;
    private readonly IToaster toaster;

    private bool loaded;
    private List<Tuple<Picture, byte[]>>? collectionThumbnails;

    public CollectionViewModel(AstroPicModel astroPicModel, IToaster toaster)
    {
        this.astroPicModel = astroPicModel;
        this.toaster = toaster;
        this.PictureViewModel = new PictureViewModel();
        this.DropViewModel = new DropViewModel();
        this.ThumbnailsPanelViewModel = new ThumbnailsPanelViewModel(this);
        this.Messenger.Subscribe<ToolbarCommandMessage>(this.OnToolbarCommand);
        this.Messenger.Subscribe<ModelLoadedMessage>(this.OnModelLoaded);
    }

    public override void Activate(object? activationParameters) 
    {
        base.Activate(activationParameters);
        if (this.loaded)
        {
            Schedule.OnUiThread(
                200,
                () => { this.ThumbnailsPanelViewModel.UpdateSelection(); },
                DispatcherPriority.Background);
        }
    }

    private void OnModelLoaded(ModelLoadedMessage message)
    {
        if (!this.loaded)
        {
            this.collectionThumbnails = this.astroPicModel.LoadCollectionThumbnails();
            this.loaded = true;
            this.ThumbnailsPanelViewModel.LoadThumnails(this.collectionThumbnails);
        }
        else
        {
            // This needs to be scheduled because virtualization will most likely 'shake' and renew
            // the bindings of views and previously bound view models 
            Schedule.OnUiThread(
                200,
                () => { this.ThumbnailsPanelViewModel.UpdateSelection(); },
                DispatcherPriority.Background);
        }
    }

    private void OnToolbarCommand(ToolbarCommandMessage message)
    {
        switch (message.Command)
        {
            case ToolbarCommandMessage.ToolbarCommand.SetWallpaper:
                this.PictureViewModel.SetWallpaper();
                break;

            case ToolbarCommandMessage.ToolbarCommand.RemoveFromCollection:
                this.PictureViewModel.RemoveFromCollection();
                break;

            case ToolbarCommandMessage.ToolbarCommand.SaveToDesktop:
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
                Provider = ProviderKey.Personal,
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
        this.PictureViewModel.Title = "Bad picture!";
        this.Logger.Warning("Bad picture!"); 
    }

    public ThumbnailsPanelViewModel ThumbnailsPanelViewModel
    {
        get => this.Get<ThumbnailsPanelViewModel?>() ?? throw new ArgumentNullException("ThumbnailsPanelViewModel");
        set => this.Set(value);
    }

    public DropViewModel DropViewModel
    {
        get => this.Get<DropViewModel?>() ?? throw new ArgumentNullException("DropViewModel");
        set => this.Set(value);
    }

    public PictureViewModel PictureViewModel
    {
        get => this.Get<PictureViewModel?>() ?? throw new ArgumentNullException("PictureViewModel");
        set => this.Set(value);
    }
}
