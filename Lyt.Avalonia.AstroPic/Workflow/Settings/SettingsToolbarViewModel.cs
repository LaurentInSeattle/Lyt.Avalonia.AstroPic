namespace Lyt.Avalonia.AstroPic.Workflow.Settings;

using static MessagingExtensions;
using static ToolbarCommandMessage;

public sealed class SettingsToolbarViewModel : Bindable<SettingsToolbarView>
{
#pragma warning disable IDE0079 
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable CA1822 // Mark members as static

    private void OnCleanup(object? _) => Command(ToolbarCommand.Cleanup);

    public ICommand CleanupCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

#pragma warning restore CA1822
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning restore IDE0079

}
