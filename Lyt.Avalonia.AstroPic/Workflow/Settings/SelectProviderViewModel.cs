namespace Lyt.Avalonia.AstroPic.Workflow.Settings;

public sealed partial class SelectProviderViewModel : ViewModel<SelectProviderView>
{
    private readonly AstroPicModel astroPicModel;
    private readonly Provider provider;
    private readonly bool isInitializing;

    [ObservableProperty]
    private string? providerName;

    [ObservableProperty]
    private bool useService;

    public SelectProviderViewModel(AstroPicModel astroPicModel, Provider provider)
    {
        this.astroPicModel = astroPicModel;
        this.provider = provider;
        this.ProviderName = this.provider.Name;
        this.isInitializing = true;
        this.UseService = this.provider.IsSelected;
        this.isInitializing = false;
    }

    partial void OnUseServiceChanged(bool value)
    {
        if (!this.isInitializing)
        {
            this.astroPicModel.UpdateProviderSelected(this.provider, isSelected: value);
        }
    }
}
