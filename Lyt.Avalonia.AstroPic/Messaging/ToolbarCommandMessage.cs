namespace Lyt.Avalonia.AstroPic.Messaging;

public sealed record class ToolbarCommandMessage(
    ToolbarCommandMessage.ToolbarCommand Command, object? CommandParameter = null)
{
    public enum ToolbarCommand
    {
        // Main tool bar 
        Today,
        Collection,
        Settings,
        About, 
        ToTray, 
        Close, 

        // Gallery toolbar 
        SetWallpaper,
        AddToCollection,
        SaveToDesktop,
    }
}
