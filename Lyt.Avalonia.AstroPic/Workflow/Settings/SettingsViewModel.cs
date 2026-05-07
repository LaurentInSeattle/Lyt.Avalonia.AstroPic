namespace Lyt.Avalonia.AstroPic.Workflow.Settings;

public sealed partial class SettingsViewModel : ViewModel<SettingsView>, IRecipient<ToolbarCommandMessage>
{
    private readonly AstroPicModel astroPicModel;

    [ObservableProperty]
    private ObservableCollection<SelectProviderViewModel> selectProviders;

    [ObservableProperty]
    public partial decimal? MaxImages { get; set; }

    [ObservableProperty]
    public partial decimal? MaxStorageMB { get; set; }

    [ObservableProperty]
    public partial bool MaxImageWidth { get; set; }

    [ObservableProperty]
    public partial bool ShouldAutoCleanup { get; set; }

    [ObservableProperty]
    public partial bool ShouldAutoStart { get; set; }

    [ObservableProperty]
    public partial bool ShouldRotateWallpapers { get; set; }

    [ObservableProperty]
    public partial decimal? WallpaperRotationMinutes { get; set; }

    private bool isPopulating;

    public SettingsViewModel(AstroPicModel astroPicModel)
    {
        this.astroPicModel = astroPicModel;
        this.SelectProviders = [];
        this.Subscribe<ToolbarCommandMessage>();
    }

    public override void OnViewLoaded()
    {
        base.OnViewLoaded();
        this.Populate();
    }

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);
        this.Populate();
    }

    public void Receive(ToolbarCommandMessage message) 
    {
        switch (message.Command)
        {
            case ToolbarCommandMessage.ToolbarCommand.Cleanup:
                this.astroPicModel.CleanupCollection(calledFromUi: true);
                break;

            // Ignore all other commands 
            default:
                break;
        }
    }

    private void Populate()
    {
        this.isPopulating = true;
        {
            lock (this.SelectProviders)
            {
                var modelProviders = this.astroPicModel.Providers;
                List<SelectProviderViewModel> providers = new(modelProviders.Count);
                foreach (Provider provider in modelProviders)
                {
                    if (provider.IsDownloadProvider)
                    {
                        providers.Add(new SelectProviderViewModel(this.astroPicModel, provider));
                    }
                }

                this.SelectProviders = [.. providers];
            }

            this.MaxImages = this.astroPicModel.MaxImages;
            this.MaxStorageMB = this.astroPicModel.MaxStorageMB;
            this.MaxImageWidth = this.astroPicModel.MaxImageWidth <= 1920;
            this.ShouldAutoCleanup = this.astroPicModel.ShouldAutoCleanup;
            this.ShouldAutoStart = this.astroPicModel.ShouldAutoStart;
            this.ShouldRotateWallpapers = this.astroPicModel.ShouldRotateWallpapers;
            this.WallpaperRotationMinutes = this.astroPicModel.WallpaperRotationMinutes;
        }

        this.isPopulating = false;
    }

    partial void OnMaxImagesChanged(decimal? value)
    {
        if (TryConvertToInt(value, out int converted))
        {
            this.astroPicModel.MaxImages = converted;
        }
    }

    partial void OnMaxStorageMBChanged(decimal? value)
    {
        if (TryConvertToInt(value, out int converted))
        {
            this.astroPicModel.MaxStorageMB = converted;
        }
    }

    partial void OnMaxImageWidthChanged(bool value)
        => this.astroPicModel.MaxImageWidth = value ? 1920 : 3840;

    partial void OnShouldAutoCleanupChanged(bool value)
        => this.astroPicModel.ShouldAutoCleanup = value;

    partial void OnShouldAutoStartChanged(bool value)
    {
        this.astroPicModel.ShouldAutoStart = value;
        if (!this.isPopulating)
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly is not null)
            {
                var autoStartService = App.GetRequiredService<IAutoStartService>();
                autoStartService.ClearAutoStart(App.Application, entryAssembly.Location);
                if (value)
                {
                    autoStartService.SetAutoStart(App.Application, entryAssembly.Location);
                }
            }
        }
    }

    partial void OnShouldRotateWallpapersChanged(bool value)
            => this.astroPicModel.ShouldRotateWallpapers = value;

    partial void OnWallpaperRotationMinutesChanged(decimal? value)
    {
        if (TryConvertToInt(value, out int converted))
        {
            this.astroPicModel.WallpaperRotationMinutes = converted;
        }
    }

    private static bool TryConvertToInt(decimal? value, out int converted)
    {
        try
        {
            converted = Convert.ToInt32(value);
            return true;
        }
        catch
        {
            // Swallow 
            converted = -1;
            return false;
        }
    }
}
