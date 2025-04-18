﻿namespace Lyt.Avalonia.AstroPic.Model;

public sealed partial class AstroPicModel : ModelBase
{
    #region Serialized -  No model changed event

    [JsonRequired]
    public string Language { get; set; } = AstroPicModel.DefaultLanguage;

    /// <summary> This should stay true, ==> But... Just FOR NOW !  </summary>
    [JsonRequired]
    public bool IsFirstRun { get; set; } = false;

    [JsonRequired]
    public int MaxImages { get => this.Get<int>(); set => this.Set(value); }

    [JsonRequired]
    public int MaxStorageMB { get => this.Get<int>(); set => this.Set(value); }

    [JsonRequired]
    public int MaxImageWidth { get => this.Get<int>(); set => this.Set(value); }

    [JsonRequired]
    public bool ShouldAutoCleanup { get => this.Get<bool>(); set => this.Set(value); }

    [JsonRequired]
    public bool ShouldAutoStart { get => this.Get<bool>(); set => this.Set(value); }

    [JsonRequired]
    public bool ShouldRotateWallpapers { get => this.Get<bool>(); set => this.Set(value); }

    [JsonRequired]
    public int WallpaperRotationMinutes { get => this.Get<int>(); set => this.Set(value); }

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

    [JsonIgnore]
    public bool ShowRecentImages { get; set; } = false;

    #endregion Not serialized - No model changed event


    #region NOT serialized - WITH model changed event

    [JsonIgnore]
    // Asynchronous: Must raise Model Updated events 
    public bool IsInternetConnected { get => this.Get<bool>(); set => this.Set(value); }

    #endregion NOT serialized - WITH model changed event    

    public void UpdateProviderSelected(Provider provider, bool isSelected)
    {
        var modelProvider =
            (from item in this.Providers
             where item.Key == provider.Key
             select item).FirstOrDefault();
        if (modelProvider is null)
        {
            return;
        }

        provider.IsSelected = isSelected;
        this.IsDirty = true;
    }
}
