namespace Lyt.Avalonia.AstroPic.Workflow.Settings;

public partial class SettingsView : UserControl, IView
{
    public SettingsView()
    {
        this.InitializeComponent();
        this.Loaded += (s, e) =>
        {
            if (this.DataContext is not null && this.DataContext is ViewModel viewModel)
            {
                viewModel.OnViewLoaded();
            }
        };
    }

}