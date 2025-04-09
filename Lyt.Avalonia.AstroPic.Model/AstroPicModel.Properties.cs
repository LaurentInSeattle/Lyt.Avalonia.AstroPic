namespace Lyt.Avalonia.AstroPic.Model;

public sealed partial class AstroPicModel : ModelBase
{
    #region Serialized -  No model changed event

    [JsonRequired]
    public string Language { get; set; } = AstroPicModel.DefaultLanguage;

    /// <summary> This should stay true, ==> But... Just FOR NOW !  </summary>
    [JsonRequired]
    public bool IsFirstRun { get; set; } = false;

    [JsonRequired]
    public bool ShouldAutoStart { get; set; } = false;

    [JsonRequired]
    public int MaxImages { get; set; } = 128;

    [JsonRequired]
    public int MaxStorageMB { get; set; } = 64;

    [JsonRequired]
    public bool ShouldRotateWallpapers { get; set; } = true ;

    [JsonRequired]
    public int WallpaperRotationMinutes { get; set; } = 5;

    [JsonRequired]
    public Dictionary<string, Picture> Pictures { get; set; } = [];

    [JsonRequired]
    public List<Provider> Providers { get; set; } = [];

    [JsonRequired]
    public Dictionary<ProviderKey, PictureMetadata> LastUpdate { get; set; } = [];

    #endregion Serialized -  No model changed event


    #region Not serialized - No model changed event

    [JsonIgnore]
    public Statistics Statistics{ get; set; } = new();

    [JsonIgnore]
    internal HashSet<string> MruWallpapers { get; set; } = [];

    [JsonIgnore]
    public Dictionary<string, byte[]> ThumbnailCache { get; set; } = [];

    [JsonIgnore]
    public bool ThumbnailsLoaded { get; set; } = false;

    [JsonIgnore]
    public bool PingComplete { get; set; } = false;

    [JsonIgnore]
    public bool ModelLoadedNotified { get; set; } = false;

    #endregion Not serialized - No model changed event

    #region NOT serialized - WITH model changed event

    [JsonIgnore]
    // Asynchronous: Must raise Model Updated events 
    public bool IsInternetConnected { get => this.Get<bool>(); set => this.Set(value); }

    #endregion NOT serialized - WITH model changed event    #region Samples 

}
