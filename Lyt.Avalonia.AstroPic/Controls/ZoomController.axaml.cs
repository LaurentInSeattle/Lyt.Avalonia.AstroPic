namespace Lyt.Avalonia.AstroPic.Controls;

public partial class ZoomController : UserControl
{
    private readonly IMessenger? messenger;

    public ZoomController()
    {
        this.InitializeComponent();
        this.Opacity = 1.0;
        this.Slider.Minimum = 1.0;
        this.Slider.Maximum = 4.0;
        this.Slider.SmallChange = 0.25;
        this.Slider.TickFrequency = 0.25;
        this.Slider.Value = 1.0;
        if (!Design.IsDesignMode)
        {
            this.messenger = App.GetRequiredService<IMessenger>();
        }
    }

    private void OnSliderValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
        => this.messenger?.Publish(new ZoomRequestMessage(e.NewValue));

    private void OnButtonMaxClick(object? sender, RoutedEventArgs e)
        => this.Slider.Value = this.Slider.Maximum;

    private void OnButtonMinClick(object? sender, RoutedEventArgs e)
        => this.Slider.Value = this.Slider.Minimum;
}