namespace Lyt.Avalonia.AstroPic.Workflow.Gallery;

using static MessagingExtensions;
using static ToolbarCommandMessage;

public sealed partial class GalleryToolbarViewModel : ViewModel<GalleryToolbarView>
{
#pragma warning disable IDE0079 
#pragma warning disable CA1822 // Mark members as static

    [RelayCommand] 
    public void OnWallpaper() => Command(ToolbarCommand.GallerySetWallpaper);
    
    [RelayCommand] 
    public void OnAddToCollection() => Command(ToolbarCommand.AddToCollection);
    
    [RelayCommand] 
    public void OnSaveToDesktop() => Command(ToolbarCommand.GallerySaveToDesktop);

#pragma warning restore CA1822
#pragma warning restore IDE0079
}
