namespace Lyt.Avalonia.AstroPic.Workflow.Settings;

public sealed class SettingsViewModel : Bindable<SettingsView>
{
    private readonly AstroPicModel astroPicModel;

    public SettingsViewModel(AstroPicModel astroPicModel)
    {
        this.astroPicModel = astroPicModel;
        this.DisablePropertyChangedLogging = true; 
    }

    public decimal? MaxImages { get => this.Get<decimal?>(); set => this.Set(value); }

    public decimal? MaxStorageMB { get => this.Get<decimal?>(); set => this.Set(value); } 

    public bool MaxImageWidth { get => this.Get<bool>(); set => this.Set(value); }

    public bool ShouldAutoCleanup { get => this.Get<bool>(); set => this.Set(value); }

    public bool ShouldAutoStart { get => this.Get<bool>(); set => this.Set(value); }

    public bool ShouldRotateWallpapers { get => this.Get<bool>(); set => this.Set(value); }

    public decimal? WallpaperRotationMinutes { get => this.Get<decimal?>(); set => this.Set(value); }
}
