namespace Lyt.Avalonia.AstroPic.Workflow.Intro;

using static ViewActivationMessage;
using static MessagingExtensions;

public sealed partial class IntroToolbarViewModel : ViewModel<IntroToolbarView>
{
#pragma warning disable IDE0079 
#pragma warning disable CA1822 // Mark members as static

    [RelayCommand]
    public void OnNext()
    {
        var astroPicModel = App.GetRequiredService<AstroPicModel>();
        astroPicModel.IsFirstRun = false;
        astroPicModel.Save();

        bool programmaticNavigation = true; 
        ActivateView(ActivatedView.Collection, programmaticNavigation);
    }

#pragma warning restore CA1822 // Mark members as static
#pragma warning restore IDE0079
}
