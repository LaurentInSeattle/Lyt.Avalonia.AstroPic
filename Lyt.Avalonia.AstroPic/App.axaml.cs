namespace Lyt.Avalonia.AstroPic;

using static ToolbarCommandMessage;
using static ViewActivationMessage;

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
            typeof(AstroPicModel),
        ],
        [
           // Singletons
           typeof(ShellViewModel),
           typeof(GalleryViewModel),
           typeof(GalleryToolbarViewModel),
           typeof(CollectionViewModel),
           typeof(CollectionToolbarViewModel),
           typeof(SettingsViewModel),
           typeof(SettingsToolbarViewModel),
           typeof(IntroViewModel),
           typeof(IntroToolbarViewModel),

           typeof(AstroPicService),
        ],
        [
            // Services 
            App.LoggerService,
            App.OsSpecificWallpaperService(),
            new Tuple<Type, Type>(typeof(IAnimationService), typeof(AnimationService)),
            new Tuple<Type, Type>(typeof(ILocalizer), typeof(LocalizerModel)),
            new Tuple<Type, Type>(typeof(IDialogService), typeof(DialogService)),
            new Tuple<Type, Type>(typeof(IMessenger), typeof(Messenger)),
            new Tuple<Type, Type>(typeof(IProfiler), typeof(Profiler)),
            new Tuple<Type, Type>(typeof(IToaster), typeof(Toaster)),
        ],
        singleInstanceRequested: false,
        splashImageUri: null,
        appSplashWindow: new SplashWindow()
        )
    {
        // This should be empty, use the OnStartup override
    }

    private static Tuple<Type, Type> LoggerService =>
            Debugger.IsAttached ?
                new Tuple<Type, Type>(typeof(ILogger), typeof(LogViewerWindow)) :
                new Tuple<Type, Type>(typeof(ILogger), typeof(Logger));

    private static Tuple<Type, Type> OsSpecificWallpaperService()
        => App.OsSpecificService<IWallpaperService>("WallpaperService");

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

        // TODO: FIX that ! ( Configure should be part of the Interface ) 
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

        this.SetupTrayIcon();

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
            if ((process is not null) && (process.MainModule is not null))
            {
                Process.Start(process.MainModule.FileName);
            }
        }

        return Task.CompletedTask;
    }

    // Why does it need to be there ??? 
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    private void SetupTrayIcon()
    {
        var icons = new TrayIcons
        {
            new TrayIcon
            {
                Icon = new WindowIcon(
                    new Bitmap(AssetLoader.Open(
                        new Uri("avares://Lyt.Avalonia.AstroPic/Assets/Images/AstroPic.ico")))),
                Menu =
                [
                    new NativeMenuItem("Apri la Collezione")
                    {
                         Command = new Command (this.OpenCollectionFromTray )
                    },
                    new NativeMenuItemSeparator(),
                    new NativeMenuItem("Impostazioni")
                    {
                         Command = new Command (this.OpenSettingsFromTray )
                    },
                ]
            }
        };

        TrayIcon.SetIcons(this, icons);
    }

    public static void ShowMainWindow(bool show = true)
    {
        Window mainWindow = App.MainWindow; 
        if ( show )
        {
            mainWindow.Show();
        }
        else
        {
            mainWindow.Hide();
        }

        mainWindow.ShowInTaskbar = show;
    }

    private void OpenCollectionFromTray(object? _)
    {
        ShowMainWindow();
        NavigateTo(ActivatedView.Collection);
    }

    private void OpenSettingsFromTray(object? _)
    {
        ShowMainWindow();
        NavigateTo(ActivatedView.Settings);
    }

    private static void NavigateTo(ActivatedView view)
    {
        var messenger = App.GetRequiredService<IMessenger>();
        messenger.Publish(new ViewActivationMessage(view));
    }
}
