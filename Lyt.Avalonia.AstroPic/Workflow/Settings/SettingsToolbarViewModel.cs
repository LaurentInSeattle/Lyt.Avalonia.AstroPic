namespace Lyt.Avalonia.AstroPic.Workflow.Settings;

using static MessagingExtensions;
using static ToolbarCommandMessage;

public sealed partial class SettingsToolbarViewModel : ViewModel<SettingsToolbarView>
{
#pragma warning disable IDE0079 
#pragma warning disable CA1822 // Mark members as static

    [RelayCommand]
    public void OnCleanup() => Command(ToolbarCommand.Cleanup);

#pragma warning restore CA1822
#pragma warning restore IDE0079

}
