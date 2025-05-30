namespace Lyt.Avalonia.AstroPic.Model.DataObjects;

public sealed record class Provider
(
    ImageProviderKey Key = ImageProviderKey.Unknown, 
    string Name = "" )
{
    public bool IsSelected { get; set; } = true ;

    public bool IsLoaded { get; set; }  

    public bool IsDownloadProvider 
        => this.Key != ImageProviderKey.Unknown && this.Key != ImageProviderKey.Personal;
}
