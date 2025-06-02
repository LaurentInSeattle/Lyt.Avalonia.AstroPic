namespace Lyt.Avalonia.AstroPic.Shell;

public partial class ImageInfoWindow : Window
{
    private DispatcherTimer? closeTimer;
   
    public ImageInfoWindow() 
    { 
        this.InitializeComponent();
        this.closeTimer = new DispatcherTimer()
        {
            Interval = TimeSpan.FromSeconds(8),
        };

        this.closeTimer.Tick += this.OnCloseTimerTick;
        this.Grid.PointerPressed += this.OnGridPointerPressed;
        this.closeTimer.Start();
    }

    private void OnGridPointerPressed(object? sender, PointerPressedEventArgs e) => this.DoClose();

    private void OnCloseTimerTick(object? _, EventArgs e) => this.DoClose();

    private void DoClose()
    { 
        if (this.closeTimer is not null)
        {
            this.closeTimer.Stop();
            this.closeTimer.Tick -= this.OnCloseTimerTick;
            this.closeTimer = null;
        }

        this.Close();
    }

    public void Update (WallpaperInfo wallpaperInfo)
    {
        this.TitleText.Text = wallpaperInfo.Title;
        this.DescriptionText.Text = wallpaperInfo.Description;
    }
}