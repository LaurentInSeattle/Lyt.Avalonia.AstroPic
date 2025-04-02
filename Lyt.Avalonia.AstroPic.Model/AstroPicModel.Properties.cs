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
    public Dictionary<string, Picture> Pictures { get; set; } = [];

    [JsonRequired]
    public List<Provider> Providers { get; set; } = [];

    [JsonRequired]
    public Dictionary<ProviderKey, PictureMetadata> LastUpdate { get; set; } = [];

    #endregion Serialized -  No model changed event


    #region Not serialized - No model changed event
    
    [JsonIgnore]
    internal HashSet<string> MruWallpapers { get; set; } = [];

    #endregion Not serialized - No model changed event


    #region Samples 

    //[JsonIgnore]
    //// Not serialized -  With model changed event
    //// public Group? SelectedGroup { get => this.Get<Group?>(); set => this.Set(value); }

    #endregion Samples 
}
