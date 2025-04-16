namespace Lyt.Avalonia.AstroPic.Messaging;

public sealed record class ToolbarCommandMessage(
    ToolbarCommandMessage.ToolbarCommand Command, object? CommandParameter = null)
{
    public enum ToolbarCommand
    {
        // Left - Main toolbar in Shell view 
        Today,
        Collection,
        Settings,
        About,

        // Right - Main toolbar in Shell view  
        ToTray, 
        Close, 

        // Gallery and Collection toolbars 
        SetWallpaper,
        AddToCollection,        // Gallery Only
        RemoveFromCollection,   // Collection Only
        SaveToDesktop,
        Cleanup,
    }
}
