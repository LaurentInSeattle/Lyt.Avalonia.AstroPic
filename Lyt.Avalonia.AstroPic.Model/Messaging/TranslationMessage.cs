namespace Lyt.Avalonia.AstroPic.Model.Messaging; 

public sealed class TranslationMessage(string title = "", string description= "")
{
    public string Title { get; set; } = title;
    public string Description { get; set ; } = description;
}