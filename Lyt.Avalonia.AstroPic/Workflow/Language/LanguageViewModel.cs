namespace Lyt.Avalonia.AstroPic.Workflow.Language;

public sealed class LanguageViewModel : Bindable<LanguageView>
{
    private static readonly List<LanguageInfoViewModel> SupportedLanguages =
    [
        new LanguageInfoViewModel("uk-UA", "Українська мова" , "Ukraine.png", string.Empty) ,
        new LanguageInfoViewModel("it-IT", "Italiano" , "Italy.png" , "San_Marino.png" ) ,
        new LanguageInfoViewModel("fr-FR", "Français" , "France.png" , "Quebec.png" ) ,
        new LanguageInfoViewModel("en-US", "English" , "United_Kingdom.png" , "Canada.png" ) ,
        new LanguageInfoViewModel("es-ES", "Español" , "Spain.png" , "Mexico.png" ) ,
        new LanguageInfoViewModel("bg-BG", "Български език" , "Bulgaria.png" , string.Empty ) ,
    ];

    private readonly AstroPicModel astroPicModel;
    private bool isInitializing; 

    public LanguageViewModel(AstroPicModel astroPicModel)
    {
        this.astroPicModel = astroPicModel;
        this.isInitializing = true;
        this.Languages = [.. SupportedLanguages];
        this.isInitializing = false;
    }

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);
        string key = this.astroPicModel.Language;
        int index = 0;
        for (int i = 0; i < this.Languages.Count; ++i)
        {
            if (key == this.Languages[i].Key)
            {
                index = i;
                break;
            } 
        }

        this.isInitializing = true;
        this.SelectedLanguageIndex = index;
        this.isInitializing = false;
    }

    public int SelectedLanguageIndex 
    { 
        get => this.Get<int>();
        set
        {
            // Update the UI...
            this.Set(value);

            // ... But do not change the language when initializing 
            if (this.isInitializing)
            {
                return; 
            } 

            string languageKey = this.Languages[value].Key; 
            Debug.WriteLine("Selected language: " + languageKey);
            this.astroPicModel.SelectLanguage (languageKey);
        } 
    }
    
    public ObservableCollection<LanguageInfoViewModel> Languages
    {
        get => this.Get<ObservableCollection<LanguageInfoViewModel>?>() ?? throw new ArgumentNullException("Languages");
        set => this.Set(value);
    }
}
