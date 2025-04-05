namespace Lyt.Avalonia.AstroPic.Workflow.Collection;

public partial class DropView : UserControl
{
    public DropView()
    {
        this.InitializeComponent();
        this.DataContextChanged += this.OnDataContextChanged; 
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (this.DataContext is DropViewModel dropViewModel)
        {
            dropViewModel.BindOnDataContextChanged(this);
        }
    }
}