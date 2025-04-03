namespace Lyt.Avalonia.AstroPic.Workflow.Gallery;

using global::Avalonia.Controls;
using static Avalonia.Controls.Utilities;

public partial class ThumbnailView : UserControl
{
    private static readonly SolidColorBrush normalBrush;
    private static readonly SolidColorBrush hotBrush;
    private static readonly SolidColorBrush selectedBrush;

    private bool isSelected;
    private bool isInside;
    private bool isHot;

    static ThumbnailView()
    {
        normalBrush = FindResource<SolidColorBrush>("LightAqua_1_100");
        hotBrush = FindResource<SolidColorBrush>("PastelOrchid_1_100");
        selectedBrush = FindResource<SolidColorBrush>("OrangePeel_0_100");
    }

    public ThumbnailView()
    {
        this.InitializeComponent();
        this.PointerEntered += this.OnPointerEnter;
        this.PointerExited += this.OnPointerLeave;
        this.PointerPressed += this.OnPointerPressed;
        this.PointerReleased += this.OnPointerReleased;
        this.DataContextChanged += this.OnDataContextChanged;
        this.SetVisualState();
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (this.DataContext is ThumbnailViewModel thumbnailViewModel)
        {
            thumbnailViewModel.BindOnDataContextChanged(this);
        }
    }

    ~ThumbnailView()
    {
        this.PointerEntered -= this.OnPointerEnter;
        this.PointerExited -= this.OnPointerLeave;
        this.PointerPressed -= this.OnPointerPressed;
        this.PointerReleased -= this.OnPointerReleased;
        this.DataContextChanged -= this.OnDataContextChanged;
    }

    public void Select()
    {
        this.isHot = false;
        this.isInside = false;
        this.isSelected = true;
        this.SetVisualState();
    }

    public void Deselect()
    {
        this.isHot = false;
        this.isInside = false;
        this.isSelected = false;
        this.SetVisualState();
    }

    private void OnPointerEnter(object? sender, PointerEventArgs args)
    {
        if ((sender is ThumbnailView view) && (this == view))
        {
            this.isInside = true;
            this.SetVisualState();
        }
    }

    private void OnPointerLeave(object? sender, PointerEventArgs args)
    {
        if ((sender is ThumbnailView view) && (this == view))
        {
            this.isInside = false;
            this.SetVisualState();
        }
    }

    private void OnPointerPressed(object? sender, PointerEventArgs args)
    {
        if ((sender is ThumbnailView view) && (this == view))
        {
            this.isHot = true;
            this.SetVisualState();
        }
    }

    private void OnPointerReleased(object? sender, PointerEventArgs args)
    {
        bool wasInside = this.isInside;
        if ((sender is ThumbnailView view) && (this == view))
        {
            this.isHot = false;
            this.isInside = false;
            this.isSelected = true;
            this.SetVisualState();
            if (wasInside && this.DataContext is ThumbnailViewModel thumbnailViewModel)
            {
                thumbnailViewModel.OnSelect();
            }
        }
    }

    private void SetVisualState()
    {
        bool visible = this.isInside || this.isSelected;
        this.outerBorder.BorderThickness = new Thickness(visible ? 1.0 : 0.0);
        this.outerBorder.BorderBrush =
            this.isHot ?
                hotBrush :
                this.isSelected ? selectedBrush : normalBrush;
    }

}