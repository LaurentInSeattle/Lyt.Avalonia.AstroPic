namespace Lyt.Avalonia.AstroPic.Workflow.Gallery;

public class GalleryViewModel : Bindable<GalleryView>
{

    public double ZoomFactor { get => this.Get<double>(); set => this.Set(value); }
}
