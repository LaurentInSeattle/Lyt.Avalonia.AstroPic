namespace Lyt.Avalonia.AstroPic.Workflow.Collection;

public sealed class CollectionViewModel : Bindable<CollectionView>
{
    private readonly AstroPicModel astroPicModel;
    private readonly IToaster toaster;

    public CollectionViewModel(AstroPicModel astroPicModel, IToaster toaster)
    {
        this.astroPicModel = astroPicModel;
        this.toaster = toaster;
        this.PictureViewModel = new PictureViewModel();
        this.DropViewModel = new DropViewModel();
        this.ThumbnailsPanelViewModel = new ThumbnailsPanelViewModel(this);
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

    internal void Select(PictureMetadata pictureMetadata, byte[] imageBytes)
        => this.PictureViewModel.Select(pictureMetadata, imageBytes);
    
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
