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
    public bool ShouldAutoStart { get; set; } = false;

    [JsonRequired]
    public int  MaxImages { get; set; } = 128;

    [JsonRequired]
    public int MaxStorageMB { get; set; } = 64;

    [JsonRequired]
    public PictureCollection Collection { get; set; } = new();

    #endregion Serialized -  No model changed event

    #region Samples 

    //[JsonIgnore]
    //// Not serialized -  With model changed event
    //// public Group? SelectedGroup { get => this.Get<Group?>(); set => this.Set(value); }

    //[JsonIgnore]
    //// Not serialized - No model changed event
    //// public List<string> AvailableIcons { get; set; } = [];

    #endregion Samples 
}
