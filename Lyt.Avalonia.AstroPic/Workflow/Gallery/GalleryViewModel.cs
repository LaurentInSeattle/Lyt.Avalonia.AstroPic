namespace Lyt.Avalonia.AstroPic.Workflow.Gallery;

public sealed partial class GalleryViewModel : ViewModel<GalleryView>
{
    private readonly AstroPicModel astroPicModel;
    private readonly IToaster toaster;

    [ObservableProperty]
    private string progressMessage;

    [ObservableProperty]
    private ThumbnailsPanelViewModel thumbnailsPanelViewModel;

    [ObservableProperty]
    private PictureViewModel pictureViewModel;

    private bool downloaded;

    public GalleryViewModel(AstroPicModel astroPicModel, IToaster toaster)
    {
        this.astroPicModel = astroPicModel;
        this.toaster = toaster;
        this.progressMessage =  string.Empty;
        this.PictureViewModel = new PictureViewModel(this);
        this.ThumbnailsPanelViewModel = new ThumbnailsPanelViewModel(this);
        this.Messenger.Subscribe<ServiceProgressMessage>(this.OnDownloadProgress, withUiDispatch: true);
        this.Messenger.Subscribe<ServiceErrorMessage>(this.OnDownloadError, withUiDispatch: true);
        this.Messenger.Subscribe<ToolbarCommandMessage>(this.OnToolbarCommand);
        this.Messenger.Subscribe<ModelLoadedMessage>(this.OnModelLoaded);
    }

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);
        if (this.downloaded)
        {
            Schedule.OnUiThread(
                200,
                () => { this.ThumbnailsPanelViewModel.UpdateSelection(); },
                DispatcherPriority.Background);
        }
    }

    private void OnModelLoaded(ModelLoadedMessage message)
    {
        if (!this.downloaded)
        {
            _ = this.DownloadImages();
        }
    }

    private void OnToolbarCommand(ToolbarCommandMessage message)
    {
        switch (message.Command)
        {
            case ToolbarCommandMessage.ToolbarCommand.GallerySetWallpaper:
                this.PictureViewModel.SetWallpaper();
                break;

            case ToolbarCommandMessage.ToolbarCommand.AddToCollection:
                this.PictureViewModel.AddToCollection();
                break;

            case ToolbarCommandMessage.ToolbarCommand.GallerySaveToDesktop:
                this.PictureViewModel.SaveToDesktop();
                break;

            // Ignore all other commands 
            default:
                break;
        }
    }

    public async Task DownloadImages()
    {
        List<PictureDownload> downloads = await this.astroPicModel.DownloadTodayImages();
        if ((downloads != null) && (downloads.Count > 0))
        {
            this.downloaded = true;
            Dispatch.OnUiThread(() => { this.LoadImages(downloads); });
        }
    }

    private void OnDownloadError(ServiceErrorMessage message)
    {
        var provider = this.astroPicModel.MaybeProviderFromKey(message.Provider);
        if (provider is null)
        {
            // Should never happen 
            if (Debugger.IsAttached) { Debugger.Break(); }
            return;
        }

        string providerLocalized = this.Localize(provider.Name);
        string errorLocalized = this.Localize(message.ErrorKey);
        this.toaster.Dismiss();
        this.toaster.Show(
            providerLocalized, errorLocalized, 10_000, InformationLevel.Warning);
    }

    private void OnDownloadProgress(ServiceProgressMessage message)
    {
        string start = message.IsBegin ?
            this.Localize("Gallery.StartingDownloading") :
            this.Localize("Gallery.CompletedDownloading");
        string middle = message.IsMetadata ?
            this.Localize("Gallery.ImageMetadata") :
            this.Localize("Gallery.Image");
        string provider = message.Provider.ToString().BeautifyEnumString();
        string end = this.Localize("Gallery.ForProvider");
        this.ProgressMessage = string.Concat(start, " ", middle, " ", end, " ", provider);
    }

    private void LoadImages(List<PictureDownload> downloads)
    {
        this.ThumbnailsPanelViewModel.LoadImages(downloads);
        Schedule.OnUiThread(
            200, () =>
            {
                string msg = this.Localize("Gallery.DownloadsComplete");
                this.ProgressMessage = msg;
            }, DispatcherPriority.Background);
    }

    internal void Select(PictureMetadata pictureMetadata, byte[] imageBytes)
    {
        this.PictureViewModel.Select(pictureMetadata, imageBytes);
        this.ProgressMessage = string.Empty;
    }
}
