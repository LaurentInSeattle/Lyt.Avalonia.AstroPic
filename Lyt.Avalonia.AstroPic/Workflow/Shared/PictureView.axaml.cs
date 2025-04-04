namespace Lyt.Avalonia.AstroPic.Workflow.Shared;

public partial class PictureView : UserControl
{
    public PictureView()
    {
        this.InitializeComponent();
        this.DataContextChanged += this.OnDataContextChanged; 
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (this.DataContext is PictureViewModel pictureViewModel)
        {
            pictureViewModel.BindOnDataContextChanged(this);
        }
    }
}