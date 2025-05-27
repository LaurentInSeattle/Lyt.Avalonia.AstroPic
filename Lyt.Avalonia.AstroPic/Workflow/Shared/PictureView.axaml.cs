namespace Lyt.Avalonia.AstroPic.Workflow.Shared;

public partial class PictureView : UserControl, IView
{
    public PictureView()
    {
        this.InitializeComponent();
        this.DataContextChanged += (s, e) =>
        {
            if (this.DataContext is PictureViewModel pictureViewModel)
            {
                pictureViewModel.BindOnDataContextChanged(this);
            }
        };

        this.Loaded += (s, e) =>
        {
            if (this.DataContext is not null && this.DataContext is ViewModel viewModel)
            {
                viewModel.OnViewLoaded();
            }
        };
    }
}