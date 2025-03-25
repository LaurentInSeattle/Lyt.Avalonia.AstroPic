namespace Lyt.Avalonia.AstroPic;

public partial class App : ApplicationBase
{
    public const string Organization = "Lyt";
    public const string Application = "AstroPic";
    public const string RootNamespace = "Lyt.Avalonia.AstroPic";
    public const string AssemblyName = "Lyt.Avalonia.AstroPic";
    public const string AssetsFolder = "Assets";

    public App() : base(
        App.Organization,
        App.Application,
        App.RootNamespace,
        typeof(MainWindow),
        typeof(ApplicationModelBase), // Top level model 
        [
            // Models 
            typeof(FileManagerModel),
        ],
        [
           // Singletons
           typeof(ShellViewModel),
           //typeof(GroupViewModel),
           //typeof(NewEditGroupViewModel),
           //typeof(HelpViewModel),
           //typeof(SettingsViewModel),
           //typeof(NewEditTemplateViewModel),
        ],
        [
            // Services 
#if DEBUG
            new Tuple<Type, Type>(typeof(ILogger), typeof(LogViewerWindow)),
#else
            new Tuple<Type, Type>(typeof(ILogger), typeof(Logger)),
#endif
            
            new Tuple<Type, Type>(typeof(ILocalizer), typeof(LocalizerModel)),
            new Tuple<Type, Type>(typeof(IDialogService), typeof(DialogService)),
            new Tuple<Type, Type>(typeof(IMessenger), typeof(Messenger)),
            new Tuple<Type, Type>(typeof(IProfiler), typeof(Profiler)),
            new Tuple<Type, Type>(typeof(IToaster), typeof(Toaster)),
        ],
        singleInstanceRequested: false)
    {
        // This should be empty, use the OnStartup override
    }

    public bool RestartRequired { get; set; }

    protected override async Task OnStartupBegin()
    {
        var logger = App.GetRequiredService<ILogger>();
        logger.Debug("OnStartupBegin begins");

        // This needs to complete before all models are initialized.
        var fileManager = App.GetRequiredService<FileManagerModel>();
        await fileManager.Configure(
            new FileManagerConfiguration(
                App.Organization, App.Application, App.RootNamespace, App.AssemblyName, App.AssetsFolder));

        // The localizer needs the File Manager, do not change the order.
        var localizer = App.GetRequiredService<ILocalizer>();

        // TODO: FIX that ! 
        if (localizer is LocalizerModel localizerModel)
        {
            await localizerModel.Configure(
                new LocalizerConfiguration
                {
                    AssemblyName = App.AssemblyName,
                    Languages = ["en-US", "fr-FR", "it-IT"],
                    // Use default for all other config parameters 
                });
        } 

        logger.Debug("OnStartupBegin complete");
    }

    protected override Task OnShutdownComplete()
    {
        var logger = App.GetRequiredService<ILogger>();
        logger.Debug("On Shutdown Complete");

        if (this.RestartRequired)
        {
            logger.Debug("On Shutdown Complete: Restart Required");
            var process = Process.GetCurrentProcess();
            if ( (process is not null) && (process.MainModule is not null ) )
            {
                Process.Start(process.MainModule.FileName);
            } 
        }

        return Task.CompletedTask; 
    }

    // Why does it need to be there ??? 
    public override void Initialize() => AvaloniaXamlLoader.Load(this);
}
