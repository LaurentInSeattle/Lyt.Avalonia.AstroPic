namespace Lyt.Avalonia.AstroPic;

using static ViewActivationMessage;

public partial class App : ApplicationBase
{
    private bool isShowingMainWindow; 

    private void SetupTrayIcon()
    {
        var trayIcon = new TrayIcon
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
            ],
        };

        trayIcon.Clicked += this.OnTrayIconClicked;

        var icons = new TrayIcons { trayIcon };
        TrayIcon.SetIcons(this, icons);
    }

    private void ClearTrayIcon() => TrayIcon.SetIcons(this, null);

    //     Raised when the TrayIcon is clicked. Note, this is only supported on Win32 and
    //     some Linux distributions, on OSX this event is not raised.
    private void OnTrayIconClicked(object? sender, EventArgs e)
    {
        if (this.isShowingMainWindow)
        {
            return ;
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

        NavigateTo(ActivatedView.Collection);
    }

    private void OpenSettingsFromTray(object? _)
    {
        if (!this.isShowingMainWindow)
        {
            this.ShowMainWindow();
        }

        NavigateTo(ActivatedView.Settings);
    }

    private static void NavigateTo(ActivatedView view)
    {
        bool programmaticNavigation = true ;
        var messenger = App.GetRequiredService<IMessenger>();
        messenger.Publish(new ViewActivationMessage(view, programmaticNavigation));
    }
}
