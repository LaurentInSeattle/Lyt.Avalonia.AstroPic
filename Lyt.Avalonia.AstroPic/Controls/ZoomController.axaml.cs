namespace Lyt.Avalonia.AstroPic.Controls;

public partial class ZoomController : UserControl
{
    private readonly IMessenger? messenger;

    public ZoomController()
    {
        this.InitializeComponent();
        this.Opacity = 1.0;
        this.Slider.Minimum = 1.0;
        this.Slider.Maximum = 8.0;
        this.Slider.SmallChange = 0.20;
        this.Slider.TickFrequency = 0.20;
        this.Slider.Value = 1.0;
        if (!Design.IsDesignMode)
        {
            this.messenger = App.GetRequiredService<IMessenger>();
        }
    }

    public void Min() => this.Slider.Value = this.Slider.Minimum;

    public void Max() => this.Slider.Value = this.Slider.Maximum;

    private void OnSliderValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
        => this.messenger?.Publish(new ZoomRequestMessage(e.NewValue, this.Tag));

    private void OnButtonMaxClick(object? sender, RoutedEventArgs e) => this.Max();

    private void OnButtonMinClick(object? sender, RoutedEventArgs e) => this.Min();

    /// <summary> MaxText Styled Property </summary>
    public static readonly StyledProperty<string> MaxTextProperty =
        AvaloniaProperty.Register<ZoomController, string>(
            nameof(MaxText),
            defaultValue: "Max",
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: null,
            enableDataValidation: false);

    /// <summary> Gets or sets the Text property.</summary>
    public string MaxText
    {
        get => this.GetValue(MaxTextProperty);
        set
        {
            this.SetValue(MaxTextProperty, value);
            this.maxButton.Text = value;
        }
    }

    /// <summary> MinText Styled Property </summary>
    public static readonly StyledProperty<string> MinTextProperty =
        AvaloniaProperty.Register<ZoomController, string>(
            nameof(MinText),
            defaultValue: "Fit",
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: null,
            enableDataValidation: false);

    /// <summary> Gets or sets the MinText property.</summary>
    public string MinText
    {
        get => this.GetValue(MinTextProperty);
        set
        {
            this.SetValue(MinTextProperty, value);
            this.minButton.Text = value;
        }
    }
}