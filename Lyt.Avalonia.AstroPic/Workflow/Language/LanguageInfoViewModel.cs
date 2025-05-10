namespace Lyt.Avalonia.AstroPic.Workflow.Language;

public sealed class LanguageInfoViewModel : Bindable<LanguageInfoView>
{
    private const string UriPath = "avares://Lyt.Avalonia.AstroPic/Assets/Images/Flags/";

    public LanguageInfoViewModel(string key, string name, string flagOne, string flagTwo)
    {
        this.Key = key;
        this.Name = name;
        if (string.IsNullOrWhiteSpace(flagTwo))
        {
            this.FlagTwo = new Bitmap(AssetLoader.Open(new Uri(UriPath + flagOne)));
        } 
        else
        {
            this.FlagOne = new Bitmap(AssetLoader.Open(new Uri(UriPath + flagOne)));
            this.FlagTwo = new Bitmap(AssetLoader.Open(new Uri(UriPath + flagTwo)));
        }
    }

    public string Key { get => this.Get<string>()!; set => this.Set(value); }

    public string Name { get => this.Get<string>()!; set => this.Set(value); }

    public Bitmap FlagOne { get => this.Get<Bitmap>()!; set => this.Set(value); }

    public Bitmap FlagTwo { get => this.Get<Bitmap>()!; set => this.Set(value); }
}
