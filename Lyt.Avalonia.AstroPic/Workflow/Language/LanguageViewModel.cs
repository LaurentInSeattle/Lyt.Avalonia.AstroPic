namespace Lyt.Avalonia.AstroPic.Workflow.Language;

public sealed class LanguageViewModel : Bindable<LanguageView>
{
    private static readonly List<LanguageInfoViewModel> SupportedLanguages =
        [
        new LanguageInfoViewModel("it-IT", "Italiano" , "Italy.png" , "San_Marino.png" ) ,
        new LanguageInfoViewModel("fr-FR", "Français" , "France.png" , "Quebec.png" ) ,
        new LanguageInfoViewModel("en-US", "English" , "United_Kingdom.png" , "Canada.png" ) ,
        new LanguageInfoViewModel("es-ES", "Español" , "Spain.png" , "Mexico.png" ) ,
        ]; 

    public LanguageViewModel()
    {
        this.Languages = new (SupportedLanguages);
        // this.
    }

    public ObservableCollection<LanguageInfoViewModel> Languages
    {
        get => this.Get<ObservableCollection<LanguageInfoViewModel>?>() ?? throw new ArgumentNullException("Languages");
        set => this.Set(value);
    }
}
