namespace Lyt.Avalonia.AstroPic.Shell;

// https://stackoverflow.com/questions/385793/programmatically-start-application-on-login 
using static MessagingExtensions; 

public sealed partial class ShellViewModel : ViewModel<ShellView>
{
    private const int MinutesToMillisecs = 60 * 1_000;

    private readonly AstroPicModel astroPicModel;
    private readonly IToaster toaster;
    private readonly TimeoutTimer rotatorTimer;
    private readonly TimeoutTimer downloadRetriesTimer;

    [ObservableProperty]
    public bool mainToolbarIsVisible;

    private ViewSelector<ActivatedView>? viewSelector;
    public bool isFirstActivation;

    #region To please the XAML viewer 

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    // Should never be executed 
    public ShellViewModel()
    {
    }
#pragma warning restore CS8618 

    #endregion To please the XAML viewer 

    public ShellViewModel(AstroPicModel astroPicModel, IToaster toaster)
    {
        this.astroPicModel = astroPicModel;
        this.toaster = toaster;

        this.downloadRetriesTimer = new TimeoutTimer(this.OnDownloadRetriesTimer, 1 * MinutesToMillisecs);
        this.rotatorTimer = new TimeoutTimer(this.OnRotatorTimer, 3 * MinutesToMillisecs);
        if (this.astroPicModel.ShouldRotateWallpapers)
        {
            this.rotatorTimer.Change(this.astroPicModel.WallpaperRotationMinutes * MinutesToMillisecs);
            this.rotatorTimer.Start();
        }

        //this.Messenger.Subscribe<ViewActivationMessage>(this.OnViewActivation);
        this.Messenger.Subscribe<ToolbarCommandMessage>(this.OnToolbarCommand);
        this.Messenger.Subscribe<LanguageChangedMessage>(this.OnLanguageChanged);
    }

    private void OnLanguageChanged(LanguageChangedMessage message)
    {
        // We need to destroy and recreate the tray icon, so that it will be properly localized
        // since its native menu will not respond to dynamic property changes 
        ShellViewModel.ClearTrayIcon();
        this.SetupTrayIcon();
    }

    private void OnToolbarCommand(ToolbarCommandMessage _) => this.rotatorTimer.Reset();

    private void OnDownloadRetriesTimer()
    {
        if (this.astroPicModel.IsUpdatingTodayImagesNeeded() &&
            this.astroPicModel.IsInternetConnected)
        {
            var galleryViewModel = App.GetRequiredService<GalleryViewModel>();
            _ = galleryViewModel.DownloadImages();
        }
        else
        {
            this.downloadRetriesTimer.Stop();
        }
    }

    private void OnRotatorTimer() => this.astroPicModel.RotateWallpaper();

    public override void OnViewLoaded()
    {
        this.Logger.Debug("OnViewLoaded begins");

        base.OnViewLoaded();
        if (this.View is null)
        {
            throw new Exception("Failed to startup...");
        }

        // Select default language 
        string preferredLanguage = this.astroPicModel.Language;
        this.Logger.Debug("Language: " + preferredLanguage);
        this.Localizer.SelectLanguage(preferredLanguage);
        Thread.CurrentThread.CurrentCulture = new CultureInfo(preferredLanguage);
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(preferredLanguage);

        this.Logger.Debug("OnViewLoaded language loaded");

        // Create all statics views and bind them 
        this.SetupWorkflow();
        this.Logger.Debug("OnViewLoaded SetupWorkflow complete");


        this.SetupTrayIcon();
        this.Logger.Debug("OnViewLoaded SetupTrayIcon complete");

        // Ready 
        this.toaster.Host = this.View.ToasterHost;
        if (true)
        {
            this.toaster.Show(
                this.Localize("Shell.Ready"), this.Localize("Shell.Greetings"),
                1_600, InformationLevel.Info);
        }

        // Delay a bit the launch of the gallery so that there is time to ping 
        this.Logger.Debug("OnViewLoaded: Internet connected: " + this.astroPicModel.IsInternetConnected);
        Schedule.OnUiThread(100, this.ActivateInitialView, DispatcherPriority.Background);

        this.Logger.Debug("OnViewLoaded complete");
    }

    private async void ActivateInitialView()
    {
        this.isFirstActivation = true;

        if (this.astroPicModel.IsFirstRun)
        {
            Select(ActivatedView.Language);
        }
        else
        {
            int retries = 3;
            while (retries > 0)
            {
                this.Logger.Debug("ActivateInitialView: Internet connected: " + this.astroPicModel.IsInternetConnected);
                if (this.astroPicModel.IsInternetConnected)
                {
                    Select(ActivatedView.Gallery);
                    this.Logger.Debug("OnViewLoaded OnViewActivation complete");
                    return;
                }

                await Task.Delay(100);
                --retries;
            }
        }

        this.Logger.Debug("OnViewLoaded OnViewActivation complete");
    }

    //private void Activate<TViewModel, TControl>(bool isFirstActivation, object? activationParameters)
    //    where TViewModel : ViewModel<TControl>
    //    where TControl : Control, IView, new()

    private void SetupWorkflow()
    {
        if (this.View is not ShellView view)
        {
            throw new Exception("No view: Failed to startup...");
        }

        var galleryViewModel = App.GetRequiredService<GalleryViewModel>();
        galleryViewModel.CreateViewAndBind();
        var galleryToolbarViewModel = App.GetRequiredService<GalleryToolbarViewModel>();
        galleryToolbarViewModel.CreateViewAndBind();
        var collectionViewModel = App.GetRequiredService<CollectionViewModel>();
        collectionViewModel.CreateViewAndBind();
        var collectionToolbarViewModel = App.GetRequiredService<CollectionToolbarViewModel>();
        collectionToolbarViewModel.CreateViewAndBind();
        var introViewModel = App.GetRequiredService<IntroViewModel>();
        introViewModel.CreateViewAndBind();
        var introToolbarViewModel = App.GetRequiredService<IntroToolbarViewModel>();
        introToolbarViewModel.CreateViewAndBind();
        var languageViewModel = App.GetRequiredService<LanguageViewModel>();
        languageViewModel.CreateViewAndBind();
        var languageToolbarViewModel = App.GetRequiredService<LanguageToolbarViewModel>();
        languageToolbarViewModel.CreateViewAndBind();
        var settingsViewModel = App.GetRequiredService<SettingsViewModel>();
        settingsViewModel.CreateViewAndBind();
        var settingsToolbarViewModel = App.GetRequiredService<SettingsToolbarViewModel>();
        settingsToolbarViewModel.CreateViewAndBind();

        var selectableViews = new List<SelectableView<ActivatedView>>()
        {
            new(ActivatedView.Gallery, galleryViewModel, view.TodayButton, galleryToolbarViewModel),
            new(ActivatedView.Collection, collectionViewModel, view.CollectionButton, collectionToolbarViewModel),
            new(ActivatedView.Intro, introViewModel, view.IntroButton, introToolbarViewModel),
            new(ActivatedView.Language, languageViewModel, view.FlagButton, languageToolbarViewModel),
            new(ActivatedView.Settings, settingsViewModel, view.SettingsButton, settingsToolbarViewModel),
        };

        // Needs to be kept alive as a class member, or else callbacks will die (and wont work) 
        this.viewSelector =
            new ViewSelector<ActivatedView>(
                this.Messenger,
                this.View.ShellViewContent,
                this.View.ShellViewToolbar,
                this.View.SelectionGroup,
                selectableViews,
                this.OnViewSelected);
    }

    private void OnViewSelected(ActivatedView activatedView)
    {
        if (this.viewSelector is null)
        {
            throw new Exception("No view selector");
        }

        var newViewModel = this.viewSelector.CurrentViewModel();
        if (newViewModel is not null)
        {
            bool mainToolbarIsHidden =
                this.astroPicModel.IsFirstRun || newViewModel is IntroViewModel;
            this.MainToolbarIsVisible = !mainToolbarIsHidden;
            if (this.isFirstActivation)
            {
                this.Profiler.MemorySnapshot(newViewModel.ViewBase!.GetType().Name + ":  Activated");
            } 
        }

        this.isFirstActivation = false;
    }

#pragma warning disable IDE0079 
#pragma warning disable CA1822 // Mark members as static

    [RelayCommand]
    public void OnToday() => Select(ActivatedView.Gallery);

    [RelayCommand]
    public void OnCollection() => Select(ActivatedView.Collection);

    [RelayCommand]
    public void OnSettings() => Select(ActivatedView.Settings);

    [RelayCommand]
    public void OnInfo() => Select(ActivatedView.Intro);

    [RelayCommand]
    public void OnLanguage() => Select(ActivatedView.Language);

    [RelayCommand]
    public void OnToTray() => this.ShowMainWindow(show: false);

    [RelayCommand]
    public void OnClose() => OnExit();

    private static async void OnExit()
    {
        ClearTrayIcon();

        var application = App.GetRequiredService<IApplicationBase>();
        await application.Shutdown();
    }

#pragma warning restore CA1822
#pragma warning restore IDE0079
}
