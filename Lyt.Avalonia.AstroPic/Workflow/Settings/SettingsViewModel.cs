namespace Lyt.Avalonia.AstroPic.Workflow.Settings;

public sealed class SettingsViewModel : Bindable<SettingsView>
{
    private readonly AstroPicModel astroPicModel;

    private bool isPopulating;

    public SettingsViewModel(AstroPicModel astroPicModel)
    {
        this.DisablePropertyChangedLogging = true;

        this.astroPicModel = astroPicModel;
        this.SelectProviders = [];
        this.Messenger.Subscribe<ToolbarCommandMessage>(this.OnToolbarCommand);
    }

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();
        this.Populate();
    }

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);
        this.Populate();
    }

    private void OnToolbarCommand(ToolbarCommandMessage message)
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

    public ObservableCollection<SelectProviderViewModel> SelectProviders
    {
        get => this.Get<ObservableCollection<SelectProviderViewModel>?>() ?? throw new ArgumentNullException("ThumbnailsPanelViewModel");
        set => this.Set(value);
    }

    public decimal? MaxImages
    {
        get => this.Get<decimal?>();
        set
        {
            this.Set(value);
            if (TryConvertToInt(value, out int converted))
            {
                this.astroPicModel.MaxImages = converted;
            }
        }
    }

    public decimal? MaxStorageMB
    {
        get => this.Get<decimal?>();
        set
        {
            this.Set(value);
            if (TryConvertToInt(value, out int converted))
            {
                this.astroPicModel.MaxStorageMB = converted;
            }
        }
    }

    public bool MaxImageWidth
    {
        get => this.Get<bool>();
        set
        {
            this.Set(value);
            this.astroPicModel.MaxImageWidth = value ? 1920 : 3840;
        }
    }

    public bool ShouldAutoCleanup
    {
        get => this.Get<bool>();
        set
        {
            this.Set(value);
            this.astroPicModel.ShouldAutoCleanup = value;
        }
    }

    public bool ShouldAutoStart
    {
        get => this.Get<bool>();
        set
        {
            this.Set(value);
            this.astroPicModel.ShouldAutoStart = value;
            if (!this.isPopulating)
            {
                var entryAssembly = Assembly.GetEntryAssembly();
                if (entryAssembly is not null)
                {
                    var autoStartService = App.GetRequiredService<IAutoStartService>();
                    autoStartService.ClearAutoStart(App.Application);
                    if (value)
                    {
                        autoStartService.SetAutoStart(App.Application, entryAssembly.Location);
                    }
                }
            }
        }
    }

    public bool ShouldRotateWallpapers
    {
        get => this.Get<bool>();
        set
        {
            this.Set(value);
            this.astroPicModel.ShouldRotateWallpapers = value;
        }
    }

    public decimal? WallpaperRotationMinutes
    {
        get => this.Get<decimal?>();
        set
        {
            this.Set(value);
            if (TryConvertToInt(value, out int converted))
            {
                this.astroPicModel.WallpaperRotationMinutes = converted;
            }
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
