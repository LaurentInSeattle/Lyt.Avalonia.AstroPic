namespace Lyt.Avalonia.AstroPic.Workflow.Settings;

public partial class SelectProviderView : UserControl
{
    public SelectProviderView()
    {
        this.InitializeComponent();
        this.DataContextChanged += this.OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (this.DataContext is SelectProviderViewModel selectProviderViewModel)
        {
            selectProviderViewModel.BindOnDataContextChanged(this);
        }
    }

    ~SelectProviderView() => this.DataContextChanged -= this.OnDataContextChanged;
}