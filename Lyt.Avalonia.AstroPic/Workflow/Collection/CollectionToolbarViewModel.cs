namespace Lyt.Avalonia.AstroPic.Workflow.Collection;

using static MessagingExtensions;
using static ToolbarCommandMessage;

public sealed class CollectionToolbarViewModel : Bindable<CollectionToolbarView>
{
#pragma warning disable IDE0079 
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable CA1822 // Mark members as static

    private void OnWallpaper(object? _) => Command(ToolbarCommand.SetWallpaper);
    private void OnRemoveFromCollection(object? _) => Command(ToolbarCommand.RemoveFromCollection);
    private void OnSaveToDesktop(object? _) => Command(ToolbarCommand.SaveToDesktop);

#pragma warning restore CA1822
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning restore IDE0079

    public ICommand WallpaperCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand RemoveFromCollectionCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand SaveToDesktopCommand { get => this.Get<ICommand>()!; set => this.Set(value); }
}
