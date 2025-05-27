namespace Lyt.Avalonia.AstroPic.Workflow.Settings;

public partial class SelectProviderView : UserControl, IView
{
    public SelectProviderView()
    {
        this.InitializeComponent();
        this.DataContextChanged += (s, e) =>
        {
            if (this.DataContext is SelectProviderViewModel selectProviderViewModel)
            {
                selectProviderViewModel.BindOnDataContextChanged(this);
            }
        };
    }
}