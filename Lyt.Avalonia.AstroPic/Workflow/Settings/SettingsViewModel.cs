namespace Lyt.Avalonia.AstroPic.Workflow.Settings;

public sealed class SettingsViewModel : Bindable<SettingsView>
{
    private readonly AstroPicModel astroPicModel;

    public SettingsViewModel(AstroPicModel astroPicModel)
    {
        this.astroPicModel = astroPicModel;
        this.DisablePropertyChangedLogging = true; 
    }

    public int MaxImages { get => this.Get<int>(); set => this.Set(value); }

    public int MaxStorageMB { get => this.Get<int>(); set => this.Set(value); } 

    public bool MaxImageWidth { get => this.Get<bool>(); set => this.Set(value); }

    public bool ShouldAutoCleanup { get => this.Get<bool>(); set => this.Set(value); } 

}
