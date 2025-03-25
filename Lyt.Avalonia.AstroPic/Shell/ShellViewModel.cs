using Lyt.Avalonia.AstroPic.Service;

namespace Lyt.Avalonia.AstroPic.Shell;

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
    }

    protected async override void OnViewLoaded()
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
        // ShellViewModel.SetupWorkflow();

        this.Logger.Debug("OnViewLoaded SetupWorkflow complete");

        this.Logger.Debug("OnViewLoaded OnViewActivation complete");

        // Ready 
        this.toaster.Host = this.View.ToasterHost;
        if (true)
        {
            this.toaster.Show(
                this.localizer.Lookup("Shell.Ready"), this.localizer.Lookup("Shell.Greetings"), 
                5_000, InformationLevel.Info);
        }
        //else
        //{
        //    this.toaster.Show(
        //        this.localizer.Lookup("Shell.NoGroups.Title"), this.localizer.Lookup("Shell.NoGroups.Hint"), 
        //        10_000, InformationLevel.Warning);
        //}

        this.Logger.Debug("OnViewLoaded complete");

        //var result = await AstroPicService.GetPictures(Provider.Nasa, DateTime.Now);
        //if ((result != null) && (result.Count > 0))
        //{
        //    var picture = result[0];
        //    var bytes = await AstroPicService.DownloadPicture(picture);
        //} 
    }

    private void OnModelUpdated(ModelUpdateMessage message)
    {
        string msgProp = string.IsNullOrWhiteSpace(message.PropertyName) ? "<unknown>" : message.PropertyName;
        string msgMethod = string.IsNullOrWhiteSpace(message.MethodName) ? "<unknown>" : message.MethodName;
        this.Logger.Debug("Model update, property: " + msgProp + " method: " + msgMethod);

        //if (message.PropertyName != nameof(this.templatesModel.SelectedGroup))
        //{
        //    this.BindGroupIcons();
        //}
    }


#pragma warning disable IDE0051 // Remove unused private members
    //private void OnSettings(object? _) => this.OnViewActivation(ActivatedView.Settings);

    //private void OnAbout(object? _) => this.OnViewActivation(ActivatedView.Help);

    //private void OnNewGroup(object? _) => this.OnViewActivation(ActivatedView.NewGroup);

    //private void OnEditGroup(object? _) => this.OnViewActivation(ActivatedView.EditGroup);

#pragma warning disable CA1822 // Mark members as static
    private void OnExit(object? _) { }
#pragma warning restore CA1822 

    public ICommand SettingsCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand AboutCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand ExitCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand NewGroupCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand EditGroupCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand DeleteGroupCommand { get => this.Get<ICommand>()!; set => this.Set(value); }
}
