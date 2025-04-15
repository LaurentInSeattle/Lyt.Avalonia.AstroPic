namespace Lyt.Avalonia.AstroPic.Workflow.Settings;

public sealed class SelectProviderViewModel : Bindable<SelectProviderView>
{
    private readonly AstroPicModel astroPicModel;
    private readonly Provider provider;
    private readonly bool isInitializing; 

    public SelectProviderViewModel(AstroPicModel astroPicModel, Provider provider)
    {
        this.astroPicModel = astroPicModel;
        this.provider = provider;
        this.ProviderName = this.provider.Name;
        this.isInitializing = true; 
        this.UseService = this.provider.IsSelected;
        this.isInitializing = false;
    }

    public string? ProviderName { get => this.Get<string?>(); set => this.Set(value); }

    public bool UseService 
    { 
        get => this.Get<bool>();
        set
        {
            this.Set(value);
            if (!this.isInitializing)
            {
                this.astroPicModel.UpdateProviderSelected(this.provider, isSelected: value);
            } 
        } 
    }
}
