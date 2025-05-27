namespace Lyt.Avalonia.AstroPic.Shell;

using CommunityToolkit.Mvvm.Input;
using static MessagingExtensions;
using static ViewActivationMessage;

// https://stackoverflow.com/questions/385793/programmatically-start-application-on-login 

public sealed partial class ShellViewModel : ViewModel<ShellView>
{
    private const int MinutesToMillisecs = 60 * 1_000;

    private readonly AstroPicModel astroPicModel;
    private readonly IToaster toaster;
    private readonly TimeoutTimer rotatorTimer;
    private readonly TimeoutTimer downloadRetriesTimer;

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

        this.Messenger.Subscribe<ViewActivationMessage>(this.OnViewActivation);
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
        ShellViewModel.SetupWorkflow();
        this.Logger.Debug("OnViewLoaded SetupWorkflow complete");


        this.SetupTrayIcon();
        this.Logger.Debug("OnViewLoaded SetupTrayIcon complete");

        // Ready 
        this.toaster.Host = this.View.ToasterHost;
        if (true)
        {
            this.toaster.Show(
                this.Localizer.Lookup("Shell.Ready"), this.Localizer.Lookup("Shell.Greetings"),
                1_600, InformationLevel.Info);
        }

        // Delay a bit the launch of the gallery so that there is time to ping 
        this.Logger.Debug("OnViewLoaded: Internet connected: " + this.astroPicModel.IsInternetConnected);
        Schedule.OnUiThread(100, this.ActivateInitialView, DispatcherPriority.Background);

        this.Logger.Debug("OnViewLoaded complete");
    }

    private async void ActivateInitialView()
    {
        if (this.astroPicModel.IsFirstRun)
        {
            this.OnViewActivation(ActivatedView.Intro, parameter: null, isFirstActivation: true);
        }
        else
        {
            int retries = 3;
            while (retries > 0)
            {
                this.Logger.Debug("ActivateInitialView: Internet connected: " + this.astroPicModel.IsInternetConnected);
                if (this.astroPicModel.IsInternetConnected)
                {
                    this.OnViewActivation(ActivatedView.Gallery, parameter: null, isFirstActivation: true);
                    this.Logger.Debug("OnViewLoaded OnViewActivation complete");
                    return;
                }

                await Task.Delay(100);
                --retries;
            }
        }

        this.Logger.Debug("OnViewLoaded OnViewActivation complete");
    }

    //private void OnModelUpdated(ModelUpdateMessage message)
    //{
    //    string msgProp = string.IsNullOrWhiteSpace(message.PropertyName) ? "<unknown>" : message.PropertyName;
    //    string msgMethod = string.IsNullOrWhiteSpace(message.MethodName) ? "<unknown>" : message.MethodName;
    //    this.Logger.Debug("Model update, property: " + msgProp + " method: " + msgMethod);
    //}

    private void OnViewActivation(ViewActivationMessage message)
        => this.OnViewActivation(message.View, message.ActivationParameter, false);

    private void OnViewActivation(ActivatedView activatedView, object? parameter = null, bool isFirstActivation = false)
    {
        Bindable? CurrentViewModel()
        {
            object? currentView = this.View.ShellViewContent.Content;
            if (currentView is Control control &&
                control.DataContext is Bindable currentViewModel)
            {
                return currentViewModel;
            }

            return null;
        }

        // Navigation also reset the wallpaper rotation timer
        this.rotatorTimer.Reset();

        if (activatedView == ActivatedView.Exit)
        {
            OnExit();
        }

        if (activatedView == ActivatedView.GoBack)
        {
            // We always go back to the Intro View 
            activatedView = ActivatedView.Intro;
        }

        bool programmaticNavigation = false;
        ActivatedView hasBeenActivated = ActivatedView.Exit;
        Bindable? currentViewModel = null;
        if (parameter is bool navigationType)
        {
            programmaticNavigation = navigationType;
            currentViewModel = CurrentViewModel();
        }

        void NoToolbar() => this.View.ShellViewToolbar.Content = null;

        void SetupToolbar<TViewModel, TControl>()
            where TViewModel : Bindable<TControl>
            where TControl : Control, new()
        {
            if (this.View is null)
            {
                throw new Exception("No view: Failed to startup...");
            }

            var newViewModel = App.GetRequiredService<TViewModel>();
            this.View.ShellViewToolbar.Content = newViewModel.View;
        }

        switch (activatedView)
        {
            default:
            case ActivatedView.Gallery:
                if (!(programmaticNavigation && currentViewModel is GalleryViewModel))
                {
                    SetupToolbar<GalleryToolbarViewModel, GalleryToolbarView>();
                    this.Activate<GalleryViewModel, GalleryView>(isFirstActivation, null);
                    hasBeenActivated = ActivatedView.Gallery;
                }
                break;

            case ActivatedView.Collection:
                if (!(programmaticNavigation && currentViewModel is CollectionViewModel))
                {
                    SetupToolbar<CollectionToolbarViewModel, CollectionToolbarView>();
                    this.Activate<CollectionViewModel, CollectionView>(isFirstActivation, null);
                    hasBeenActivated = ActivatedView.Collection;
                }
                break;

            case ActivatedView.Language:
                NoToolbar();
                // this.Activate<LanguageViewModel, LanguageView>(isFirstActivation, null);
                break;

            case ActivatedView.Intro:
                SetupToolbar<IntroToolbarViewModel, IntroToolbarView>();
                this.Activate<IntroViewModel, IntroView>(isFirstActivation, null);
                break;

            case ActivatedView.Settings:
                if (!(programmaticNavigation && currentViewModel is SettingsViewModel))
                {
                    SetupToolbar<SettingsToolbarViewModel, SettingsToolbarView>();
                    this.Activate<SettingsViewModel, SettingsView>(isFirstActivation, parameter);
                    hasBeenActivated = ActivatedView.Settings;
                }
                break;
        }

        // Reflect in the navigation toolbar the programmatic change 
        if (programmaticNavigation && (hasBeenActivated != ActivatedView.Exit))
        {
            if (this.View is not ShellView view)
            {
                throw new Exception("No view: Failed to startup...");
            }

            var selector = view.SelectionGroup;
            var button = hasBeenActivated switch
            {
                ActivatedView.Intro => view.IntroButton,
                ActivatedView.Collection => view.CollectionButton,
                ActivatedView.Settings => view.SettingsButton,
                _ => view.TodayButton,
            };
            selector.Select(button);
        }

        this.MainToolbarIsVisible = CurrentViewModel() is not IntroViewModel;
    }

    private static async void OnExit()
    {
        ClearTrayIcon();

        var application = App.GetRequiredService<IApplicationBase>();
        await application.Shutdown();
    }

    private void Activate<TViewModel, TControl>(bool isFirstActivation, object? activationParameters)
        where TViewModel : Bindable<TControl>
        where TControl : Control, new()
    {
        if (this.View is null)
        {
            throw new Exception("No view: Failed to startup...");
        }

        var newViewModel = App.GetRequiredService<TViewModel>();
        object? currentView = this.View.ShellViewContent.Content;
        if (currentView is Control control && control.DataContext is Bindable currentViewModel)
        {
            if (newViewModel == currentViewModel)
            {
                return;
            }

            currentViewModel.Deactivate();
        }


        newViewModel.Activate(activationParameters);
        this.View.ShellViewContent.Content = newViewModel.View;
        if (!isFirstActivation)
        {
            this.Profiler.MemorySnapshot(newViewModel.View.GetType().Name + ":  Activated");
        }
    }

    private static void SetupWorkflow()
    {
        App.GetRequiredService<GalleryViewModel>().CreateViewAndBind();
        App.GetRequiredService<GalleryToolbarViewModel>().CreateViewAndBind();
        App.GetRequiredService<CollectionViewModel>().CreateViewAndBind();
        App.GetRequiredService<CollectionToolbarViewModel>().CreateViewAndBind();
        App.GetRequiredService<IntroViewModel>().CreateViewAndBind();
        App.GetRequiredService<IntroToolbarViewModel>().CreateViewAndBind();
        App.GetRequiredService<LanguageViewModel>().CreateViewAndBind();
        App.GetRequiredService<SettingsViewModel>().CreateViewAndBind();
        App.GetRequiredService<SettingsToolbarViewModel>().CreateViewAndBind();
    }

    [RelayCommand]
    public void OnToday() => this.OnViewActivation(ActivatedView.Gallery);

    [RelayCommand]
    public void OnCollection() => this.OnViewActivation(ActivatedView.Collection);

    [RelayCommand]
    public void OnSettings() => this.OnViewActivation(ActivatedView.Settings);

    [RelayCommand]
    public void OnInfo() => this.OnViewActivation(ActivatedView.Intro);

    [RelayCommand]
    public void OnLanguage() => this.OnViewActivation(ActivatedView.Language);

    [RelayCommand]
    public void OnToTray() => this.ShowMainWindow(show: false);

#pragma warning disable IDE0079 
#pragma warning disable CA1822 // Mark members as static

    [RelayCommand]
    public void OnClose() => OnExit();

#pragma warning restore CA1822
#pragma warning restore IDE0079

    [ObservableProperty]
    public bool mainToolbarIsVisible ; 
}
