namespace Lyt.Avalonia.AstroPic.Workflow.Language;

public sealed partial class LanguageToolbarViewModel : ViewModel<LanguageToolbarView>
{
    [RelayCommand]
    public void OnNext()
        => ViewSelector<ActivatedView>.Select(this.Messenger, ActivatedView.Intro);
}
