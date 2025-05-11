namespace Lyt.Avalonia.AstroPic.Shell;

using static ViewActivationMessage;

public sealed partial class ShellViewModel : Bindable<ShellView>
{
    private bool isShowingMainWindow;

    private void SetupTrayIcon()
    {
        // TODO: Fix that (if possible) 
        // Tray will localize only at app startup 
        // Tray will not re-localize when language is changed

        var localizer = App.GetRequiredService<ILocalizer>();
        string openCollection = localizer.Lookup("Tray.OpenCollection");
        string settings = localizer.Lookup("Tray.Settings");
        string imageInfo = localizer.Lookup("Tray.ImageInfo");
        var trayIcon = new TrayIcon
        {
            Icon = new WindowIcon(
                new Bitmap(AssetLoader.Open(
                    new Uri("avares://Lyt.Avalonia.AstroPic/Assets/Images/AstroPic.ico")))),
            Menu =
            [
                new NativeMenuItem(openCollection)
                {
                    Command = new Command (this.OpenCollectionFromTray)
                },
                new NativeMenuItemSeparator(),
                new NativeMenuItem(settings)
                {
                    Command = new Command (this.OpenSettingsFromTray)
                },
                new NativeMenuItemSeparator(),
                new NativeMenuItem(imageInfo)
                {
                    Command = new Command (this.ShowImageInfoFromTray)
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

    private void OpenCollectionFromTray(object? _)
    {
        if (!this.isShowingMainWindow)
        {
            this.ShowMainWindow();
        }

        this.NavigateTo(ActivatedView.Collection);
    }

    private void OpenSettingsFromTray(object? _)
    {
        if (!this.isShowingMainWindow)
        {
            this.ShowMainWindow();
        }

        this.NavigateTo(ActivatedView.Settings);
    }

    private void ShowImageInfoFromTray(object? _)
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

    private void NavigateTo(ActivatedView view)
    {
        bool programmaticNavigation = true;
        this.messenger.Publish(new ViewActivationMessage(view, programmaticNavigation));
    }
}

