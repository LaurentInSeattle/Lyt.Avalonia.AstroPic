namespace Lyt.Avalonia.AstroPic.Workflow.Settings;

public sealed class SettingsViewModel : Bindable<SettingsView>
{
    private readonly AstroPicModel astroPicModel;

    public SettingsViewModel(AstroPicModel astroPicModel)
    {
        this.astroPicModel = astroPicModel;
        this.SelectProviders = []; 
        this.DisablePropertyChangedLogging = true;
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

    private void Populate()
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
    } 

    public ObservableCollection<SelectProviderViewModel> SelectProviders
    {
        get => this.Get<ObservableCollection<SelectProviderViewModel>?>() ?? throw new ArgumentNullException("ThumbnailsPanelViewModel");
        set => this.Set(value);
    }

    public decimal? MaxImages { get => this.Get<decimal?>(); set => this.Set(value); }

    public decimal? MaxStorageMB { get => this.Get<decimal?>(); set => this.Set(value); } 

    public bool MaxImageWidth { get => this.Get<bool>(); set => this.Set(value); }

    public bool ShouldAutoCleanup { get => this.Get<bool>(); set => this.Set(value); }

    public bool ShouldAutoStart { get => this.Get<bool>(); set => this.Set(value); }

    public bool ShouldRotateWallpapers { get => this.Get<bool>(); set => this.Set(value); }

    public decimal? WallpaperRotationMinutes { get => this.Get<decimal?>(); set => this.Set(value); }
}
