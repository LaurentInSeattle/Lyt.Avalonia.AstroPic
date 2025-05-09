namespace Lyt.Avalonia.AstroPic.Workflow.Intro;

using static ViewActivationMessage;
using static MessagingExtensions;

public sealed class IntroToolbarViewModel : Bindable<IntroToolbarView>
{
#pragma warning disable IDE0079 
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable CA1822 // Mark members as static

    private void OnNext(object? _)
    {
        var astroPicModel = App.GetRequiredService<AstroPicModel>();
        astroPicModel.IsFirstRun = false;
        astroPicModel.Save();

        bool programmaticNavigation = true; 
        ActivateView(ActivatedView.Collection, programmaticNavigation);
    } 

    public ICommand NextCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

#pragma warning restore CA1822 // 
#pragma warning restore IDE0051
#pragma warning restore IDE0079
}
