namespace Lyt.Avalonia.AstroPic.Model.DataObjects;

public sealed record class Provider
(
    ProviderKey Key = ProviderKey.Unknown, 
    string Name = "", // string.Empty;
    bool IsSelected = true
)
{
    public bool IsLoaded { get; set; }  

    public bool IsDownloadProvider 
        => this.Key != ProviderKey.Unknown && this.Key != ProviderKey.Personal;
}
