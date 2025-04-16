namespace Lyt.Avalonia.AstroPic.Shell;

using static MessagingExtensions;
using static ViewActivationMessage;

// https://stackoverflow.com/questions/385793/programmatically-start-application-on-login 

public sealed class ShellViewModel : Bindable<ShellView>
{
    private const int MinutesToMillisecs = 60 * 1_000;

    // TODO: Cleanup this list of services when ready 
    private readonly AstroPicModel astroPicModel;
    private readonly IDialogService dialogService;
    private readonly IToaster toaster;
    private readonly IMessenger messenger;
    private readonly IProfiler profiler;
    private readonly ILocalizer localizer;
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

    public ShellViewModel(
        AstroPicModel astroPicModel,
        ILocalizer localizer,
        IDialogService dialogService, IToaster toaster, IMessenger messenger, IProfiler profiler)
    {
        this.astroPicModel = astroPicModel;
        this.localizer = localizer;
        this.dialogService = dialogService;
        this.toaster = toaster;
        this.messenger = messenger;
        this.profiler = profiler;

        this.downloadRetriesTimer = new TimeoutTimer(this.OnDownloadRetriesTimer, 1 * MinutesToMillisecs);
        this.rotatorTimer = new TimeoutTimer(this.OnRotatorTimer, 3 * MinutesToMillisecs);
        if (this.astroPicModel.ShouldRotateWallpapers)
        {
            this.rotatorTimer.Change(this.astroPicModel.WallpaperRotationMinutes * MinutesToMillisecs);
            this.rotatorTimer.Start();
        }

        this.Messenger.Subscribe<ViewActivationMessage>(this.OnViewActivation);
        this.Messenger.Subscribe<ShowTitleBarMessage>(this.OnShowTitleBar);
        this.Messenger.Subscribe<ToolbarCommandMessage>(this.OnToolbarCommand);
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

    protected override void OnViewLoaded()
    {
        this.Logger.Debug("OnViewLoaded begins");

        base.OnViewLoaded();
        if (this.View is null)
        {
            throw new Exception("Failed to startup...");
        }

        // Select default language 
        string preferredLanguage = "it-IT";
        this.Logger.Debug("Language: " + preferredLanguage);
        this.localizer.SelectLanguage(preferredLanguage);

        this.Logger.Debug("OnViewLoaded language loaded");

        // Create all statics views and bind them 
        ShellViewModel.SetupWorkflow();

        this.Logger.Debug("OnViewLoaded SetupWorkflow complete");


        // Ready 
        this.toaster.Host = this.View.ToasterHost;
        if (true)
        {
            this.toaster.Show(
                this.localizer.Lookup("Shell.Ready"), this.localizer.Lookup("Shell.Greetings"),
                5_000, InformationLevel.Info);
        }

        // Delay a bit the launch of the gallery so that there is time to ping 
        this.Logger.Debug("OnViewLoaded: Internet connected: " + this.astroPicModel.IsInternetConnected);
        Schedule.OnUiThread(100, this.ActivateInitialView, DispatcherPriority.Background);

        this.Logger.Debug("OnViewLoaded complete");
    }

    private async void ActivateInitialView()
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

        this.OnViewActivation(ActivatedView.Gallery, parameter: null, isFirstActivation: true);
        this.Logger.Debug("OnViewLoaded OnViewActivation complete");
    }

    //private void OnModelUpdated(ModelUpdateMessage message)
    //{
    //    string msgProp = string.IsNullOrWhiteSpace(message.PropertyName) ? "<unknown>" : message.PropertyName;
    //    string msgMethod = string.IsNullOrWhiteSpace(message.MethodName) ? "<unknown>" : message.MethodName;
    //    this.Logger.Debug("Model update, property: " + msgProp + " method: " + msgMethod);
    //}

    // TODO: Decide whether or not we are going to have a title bar !
    private void OnShowTitleBar(ShowTitleBarMessage message)
    {
        //this.TitleBarHeight = new GridLength(message.Show ? 42.0 : 0.0);
        //this.IsTitleBarVisible = message.Show;
    }

    private void OnViewActivation(ViewActivationMessage message)
        => this.OnViewActivation(message.View, message.ActivationParameter, false);

    private void OnViewActivation(ActivatedView activatedView, object? parameter = null, bool isFirstActivation = false)
    {
        // Navigation also reset the wallpaper rotation timer
        this.rotatorTimer.Reset();

        if (activatedView == ActivatedView.Exit)
        {
            ShellViewModel.OnExit();
        }

        if (activatedView == ActivatedView.GoBack)
        {
            // We always go back to the Intro View 
            activatedView = ActivatedView.Intro;
        }

        switch (activatedView)
        {
            default:
            case ActivatedView.Gallery:
                this.SetupToolbar<GalleryToolbarViewModel, GalleryToolbarView>();
                this.Activate<GalleryViewModel, GalleryView>(isFirstActivation, null);
                break;

            case ActivatedView.Collection:
                this.SetupToolbar<CollectionToolbarViewModel, CollectionToolbarView>();
                this.Activate<CollectionViewModel, CollectionView>(isFirstActivation, null);
                break;

            case ActivatedView.Intro:
                this.SetupToolbar<IntroToolbarViewModel, IntroToolbarView>();
                this.Activate<IntroViewModel, IntroView>(isFirstActivation, null);
                break;

            case ActivatedView.Settings:
                this.SetupToolbar<SettingsToolbarViewModel, SettingsToolbarView>();
                this.Activate<SettingsViewModel, SettingsView>(isFirstActivation, parameter);
                break;
        }
    }

    private async static void OnExit()
    {
        var application = App.GetRequiredService<IApplicationBase>();
        await application.Shutdown();
    }

    private void SetupToolbar<TViewModel, TControl>()
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
        static void CreateAndBind<TViewModel, TControl>()
             where TViewModel : Bindable<TControl>
             where TControl : Control, new()
        {
            var vm = App.GetRequiredService<TViewModel>();
            vm.CreateViewAndBind();
        }

        CreateAndBind<GalleryViewModel, GalleryView>();
        CreateAndBind<GalleryToolbarViewModel, GalleryToolbarView>();
        CreateAndBind<CollectionViewModel, CollectionView>();
        CreateAndBind<CollectionToolbarViewModel, CollectionToolbarView>();
        CreateAndBind<IntroViewModel, IntroView>();
        CreateAndBind<IntroToolbarViewModel, IntroToolbarView>();
        CreateAndBind<SettingsViewModel, SettingsView>();
        CreateAndBind<SettingsToolbarViewModel, SettingsToolbarView>();
    }

#pragma warning disable IDE0079 
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable CA1822 // Mark members as static

    private void OnToday(object? _) => this.OnViewActivation(ActivatedView.Gallery);

    private void OnCollection(object? _) => this.OnViewActivation(ActivatedView.Collection);

    private void OnSettings(object? _) => this.OnViewActivation(ActivatedView.Settings);

    private void OnInfo(object? _) => this.OnViewActivation(ActivatedView.Intro);

    private void OnToTray(object? _) => App.ShowMainWindow(show:false); 

    private void OnExit(object? _) => ShellViewModel.OnExit();

#pragma warning restore CA1822
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning restore IDE0079

    public ICommand TodayCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand CollectionCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand SettingsCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand InfoCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand ToTrayCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand ExitCommand { get => this.Get<ICommand>()!; set => this.Set(value); }
}
