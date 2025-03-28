﻿namespace Lyt.Avalonia.AstroPic.Shell;

using static MessagingExtensions;
using static ViewActivationMessage;

// https://stackoverflow.com/questions/385793/programmatically-start-application-on-login 

public sealed class ShellViewModel : Bindable<ShellView>
{
    private readonly IDialogService dialogService;
    private readonly IToaster toaster;
    private readonly IMessenger messenger;
    private readonly IProfiler profiler;
    private readonly ILocalizer localizer;

    public ShellViewModel(
        ILocalizer localizer, 
        IDialogService dialogService, IToaster toaster, IMessenger messenger, IProfiler profiler)
    {
        this.localizer = localizer;
        this.dialogService = dialogService;
        this.toaster = toaster;
        this.messenger = messenger;
        this.profiler = profiler;

        this.Messenger.Subscribe<ViewActivationMessage>(this.OnViewActivation);
        this.Messenger.Subscribe<ShowTitleBarMessage>(this.OnShowTitleBar);
    }

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

        this.OnViewActivation(ActivatedView.Gallery, parameter: null, isFirstActivation: true);
        this.Logger.Debug("OnViewLoaded OnViewActivation complete");

        // Ready 
        this.toaster.Host = this.View.ToasterHost;
        if (true)
        {
            this.toaster.Show(
                this.localizer.Lookup("Shell.Ready"), this.localizer.Lookup("Shell.Greetings"), 
                5_000, InformationLevel.Info);
        }

        this.Logger.Debug("OnViewLoaded complete");

    }

    //private void OnModelUpdated(ModelUpdateMessage message)
    //{
    //    string msgProp = string.IsNullOrWhiteSpace(message.PropertyName) ? "<unknown>" : message.PropertyName;
    //    string msgMethod = string.IsNullOrWhiteSpace(message.MethodName) ? "<unknown>" : message.MethodName;
    //    this.Logger.Debug("Model update, property: " + msgProp + " method: " + msgMethod);
    //}

    private void OnShowTitleBar(ShowTitleBarMessage message)
    {
        //this.TitleBarHeight = new GridLength(message.Show ? 42.0 : 0.0);
        //this.IsTitleBarVisible = message.Show;
    }

    private void OnViewActivation(ViewActivationMessage message)
        => this.OnViewActivation(message.View, message.ActivationParameter, false);

    private void OnViewActivation(ActivatedView activatedView, object? parameter = null, bool isFirstActivation = false)
    {
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
                this.Activate<GalleryViewModel, GalleryView>(isFirstActivation, null);
                break;

            case ActivatedView.Intro:
                // this.Activate<IntroViewModel, IntroView>(isFirstActivation, null);
                break;

            case ActivatedView.Settings:
                // this.Activate<RunViewModel, RunView>(isFirstActivation, parameter);
                break;
        }
    }

    private async static void OnExit()
    {
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

        object? currentView = this.View.ShellViewContent.Content;
        if (currentView is Control control && control.DataContext is Bindable currentViewModel)
        {
            currentViewModel.Deactivate();
        }

        var newViewModel = App.GetRequiredService<TViewModel>();
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
        // CreateAndBind<IntroViewModel, IntroView>();
        // CreateAndBind<SettingsViewModel, SettingsView>();
    }


#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable CA1822 // Mark members as static

    //private void OnSettings(object? _) => this.OnViewActivation(ActivatedView.Settings);

    private void OnExit(object? _) { }
#pragma warning restore CA1822 
#pragma warning restore IDE0051 // Remove unused private members

    public ICommand SettingsCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand ExitCommand { get => this.Get<ICommand>()!; set => this.Set(value); }
}
