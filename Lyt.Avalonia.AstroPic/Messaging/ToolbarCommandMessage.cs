namespace Lyt.Avalonia.AstroPic.Messaging;

public sealed record class ToolbarCommandMessage(
    ToolbarCommandMessage.ToolbarCommand Command, object? CommandParameter = null)
{
    public enum ToolbarCommand
    {
        // Main tool bar 
        Settings,
        Save,
        SaveToFile,
        Close, 
    }
}
