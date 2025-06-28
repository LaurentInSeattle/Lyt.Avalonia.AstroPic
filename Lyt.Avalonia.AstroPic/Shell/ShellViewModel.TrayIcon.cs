namespace Lyt.Avalonia.AstroPic.Shell;

using static MessagingExtensions;

public sealed partial class ShellViewModel : ViewModel<ShellView>
{
    private bool isShowingMainWindow;

    private void SetupTrayIcon()
    {
        string openCollection = this.Localize("Tray.OpenCollection");
        string settings = this.Localize("Tray.Settings");
        string imageInfo = this.Localize("Tray.ImageInfo");
        string exit = this.Localize("Tray.Quit");
        var trayIcon = new TrayIcon
        {
            Icon = new WindowIcon(
                new Bitmap(AssetLoader.Open(
                    new Uri("avares://Lyt.Avalonia.AstroPic/Assets/Images/AstroPic.ico")))),
            Menu =
            [
                new NativeMenuItem(openCollection)
                {
                    Command = new RelayCommand (this.OpenCollectionFromTray)
                },
                new NativeMenuItem(settings)
                {
                    Command = new RelayCommand(this.OpenSettingsFromTray)
                },
                new NativeMenuItemSeparator(),
                new NativeMenuItem(imageInfo)
                {
                    Command = new RelayCommand(this.ShowImageInfoFromTray)
                },
                new NativeMenuItemSeparator(),
                new NativeMenuItem(exit)
                {
                    Command = new RelayCommand(this.QuitFromTray)
                },
            ],
        };

        trayIcon.Clicked += this.OnTrayIconClicked;

        var icons = new TrayIcons { trayIcon };
        TrayIcon.SetIcons(App.Instance, icons);
    }

    private static void ClearTrayIcon() => TrayIcon.SetIcons(App.Instance, null);

    //     Raised when the TrayIcon is clicked. Note, this is only supported on Win32 and
    //     some Linux distributions, on OSX this event is not raised.
    private void OnTrayIconClicked(object? sender, EventArgs e)
    {
        if (this.isShowingMainWindow)
        {
            return;
        }

        // Do not navigate, just show the window if needed 
        this.ShowMainWindow();
    }

    public void ShowMainWindow(bool show = true)
    {
        Window mainWindow = App.MainWindow;

        // Would be nice to have: 
        //      show ? mainWindow.Show() : mainWindow.Hide();
        //      mainWindow. show ? Show() : Hide();
        if (show)
        {
            mainWindow.Show();
        }
        else
        {
            mainWindow.Hide();
        }

        mainWindow.ShowInTaskbar = show;
        this.isShowingMainWindow = show;
    }

    private void OpenCollectionFromTray()
    {
        if (!this.isShowingMainWindow)
        {
            this.ShowMainWindow();
        }

        Select(ActivatedView.Collection);
    }

    private void OpenSettingsFromTray()
    {
        if (!this.isShowingMainWindow)
        {
            this.ShowMainWindow();
        }

        Select(ActivatedView.Settings);
    }

    private void ShowImageInfoFromTray()
    {
        var wallpaperInfo = this.astroPicModel.WallpaperInfo;
        if (wallpaperInfo is not null)
        {
            if (!string.IsNullOrWhiteSpace(wallpaperInfo.Title))
            {
                var window = new ImageInfoWindow();
                window.Update(wallpaperInfo);
                window.Show();
            }
        } 
    }

    private void QuitFromTray()
    {
        Schedule.OnUiThread(50,
            async () =>
            {
                var application = App.GetRequiredService<IApplicationBase>();
                await application.Shutdown();
            }, DispatcherPriority.Normal);
    }
}
