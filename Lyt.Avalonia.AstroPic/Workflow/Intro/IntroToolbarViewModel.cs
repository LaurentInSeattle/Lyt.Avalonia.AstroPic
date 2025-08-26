namespace Lyt.Avalonia.AstroPic.Workflow.Intro;

public sealed partial class IntroToolbarViewModel : ViewModel<IntroToolbarView>
{
#pragma warning disable CA1822 // Mark members as static
    [RelayCommand]
    public void OnNext()
    {
        var astroPicModel = App.GetRequiredService<AstroPicModel>();
        astroPicModel.IsFirstRun = false;
        astroPicModel.Save();

        ViewSelector<ActivatedView>.Select(ActivatedView.Collection);
    }
#pragma warning restore CA1822 // Mark members as static
}
