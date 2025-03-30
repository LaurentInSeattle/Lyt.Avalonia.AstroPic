using Lyt.Avalonia.AstroPic.Interfaces;
using Lyt.Avalonia.Mvvm.Animations;

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
            typeof(AstroPicModel),
        ],
        [
           // Singletons
           typeof(ShellViewModel),
           typeof(GalleryViewModel),
           typeof(GalleryToolbarViewModel),
           //typeof(SettingsViewModel),
           //typeof(IntroViewModel),

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
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return new Tuple<Type, Type>(typeof(IWallpaperService), typeof(Windows.WallpaperService)); 
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return new Tuple<Type, Type>(typeof(IWallpaperService), typeof(MacOs.WallpaperService));
        }
        else
        {
            // OSPlatform.Linux is NOT supported, at least for now, no way to test it here
            throw new ArgumentException("Unsupported platform: " + RuntimeInformation.OSDescription);
        }
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
            if ((process is not null) && (process.MainModule is not null))
            {
                Process.Start(process.MainModule.FileName);
            }
        }

        return Task.CompletedTask;
    }

    // Why does it need to be there ??? 
    public override void Initialize() => AvaloniaXamlLoader.Load(this);
}
