using Lyt.Avalonia.AstroPic.Workflow.Shared;

namespace Lyt.Avalonia.AstroPic.Workflow.Gallery;

public sealed class GalleryViewModel : Bindable<GalleryView>
{
    private readonly AstroPicModel astroPicModel;
    private readonly IToaster toaster;

    private bool downloaded;

    public GalleryViewModel(AstroPicModel astroPicModel, IToaster toaster)
    {
        this.astroPicModel = astroPicModel;
        this.toaster = toaster;
        this.PictureViewModel = new PictureViewModel();
        this.ThumbnailsPanelViewModel = new ThumbnailsPanelViewModel(this);
        this.Messenger.Subscribe<ServiceProgressMessage>(this.OnDownloadProgress, withUiDispatch: true);
        this.Messenger.Subscribe<ServiceErrorMessage>(this.OnDownloadError, withUiDispatch: true);
        this.Messenger.Subscribe<ToolbarCommandMessage>(this.OnToolbarCommand);
    }

    private void OnToolbarCommand(ToolbarCommandMessage message)
    {
        switch (message.Command)
        {
            case ToolbarCommandMessage.ToolbarCommand.SetWallpaper:
                this.PictureViewModel.SetWallpaper();
                break;

            case ToolbarCommandMessage.ToolbarCommand.AddToCollection:
                this.PictureViewModel.AddToCollection();
                break;

            case ToolbarCommandMessage.ToolbarCommand.SaveToDesktop:
                this.PictureViewModel.SaveToDesktop();
                break;

            // Ignore all other commands 
            default:
                break;
        }
    }

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);
        if (!this.downloaded)
        {
            _ = this.DownloadImages();
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
        this.toaster.Dismiss();
        this.toaster.Show(
            message.Provider.ToString().BeautifyEnumString(),
            message.ErrorKey, 10_000, InformationLevel.Warning);
    }

    private void OnDownloadProgress(ServiceProgressMessage message)
    {
        string start = message.IsBegin ? "Starting downloading " : "Completed downloading ";
        string middle = message.IsMetadata ? " image metadata " : " image ";
        string provider = message.Provider.ToString().BeautifyEnumString();
        string end = " for provider ";
        this.ProgressMessage = string.Concat(start, middle, end, provider);
    }

    private void LoadImages(List<PictureDownload> downloads)
    {
        this.ThumbnailsPanelViewModel.LoadImages(downloads);
        Schedule.OnUiThread(
            200, () => { this.ProgressMessage = "Downloads complete!"; }, DispatcherPriority.Background);
    }

    internal void Select(PictureMetadata pictureMetadata, byte[] imageBytes)
    {
        this.PictureViewModel.Select(pictureMetadata, imageBytes);
        this.ProgressMessage = string.Empty;
    }

    public string ProgressMessage { get => this.Get<string>()!; set => this.Set(value); }

    public ThumbnailsPanelViewModel ThumbnailsPanelViewModel
    {
        get => this.Get<ThumbnailsPanelViewModel?>() ?? throw new ArgumentNullException("ThumbnailsPanelViewModel");
        set => this.Set(value);
    }

    public PictureViewModel PictureViewModel
    {
        get => this.Get<PictureViewModel?>() ?? throw new ArgumentNullException("PictureViewModel");
        set => this.Set(value);
    }
}
