namespace Lyt.Avalonia.AstroPic.Workflow.Intro;

public sealed partial class IntroToolbarViewModel : ViewModel<IntroToolbarView>
{
    [RelayCommand]
    public void OnNext()
    {
        var astroPicModel = App.GetRequiredService<AstroPicModel>();
        astroPicModel.IsFirstRun = false;
        astroPicModel.Save();

        ViewSelector<ActivatedView>.Select(this.Messenger, ActivatedView.Collection);
    }
}
