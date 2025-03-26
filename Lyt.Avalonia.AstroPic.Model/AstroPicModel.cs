using static Lyt.Avalonia.Persistence.FileManagerModel;

namespace Lyt.Avalonia.AstroPic.Model;

public class AstroPicModel : ModelBase
{
    public const string DefaultLanguage = "it-IT";
    private const string AstroPicModelFilename = "AstroPicData";

    private static readonly AstroPicModel DefaultData =
        new()
        {
            Language = DefaultLanguage,
            IsFirstRun = true,
            ShouldAutoStart = false,
        }; 
    private readonly FileManagerModel fileManager;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable IDE0021 // Use expression body for constructor 
    public AstroPicModel() : base(null, null)
    {
        // Do not inject the FileManagerModel instance: a parameter-less ctor is required for Deserialization 
        // Empty CTOR required for deserialization 
        this.ShouldAutoSave = false;
    }
#pragma warning restore IDE0021
#pragma warning restore CS8625 
#pragma warning restore CS8618

    public AstroPicModel(FileManagerModel fileManager, IMessenger messenger, ILogger logger) : base(messenger, logger)
    {
        this.fileManager = fileManager;
        this.ShouldAutoSave = true;
    }

    // Serialized -  No model changed event
    [JsonRequired]
    public string Language { get; set; } = AstroPicModel.DefaultLanguage;

    /// <summary> This should stay true, ==> But... Just FOR NOW !  </summary>
    [JsonRequired]
    public bool IsFirstRun { get; set; } = false;

    [JsonRequired]
    public bool ShouldAutoStart { get; set; } = false;

    //// Serialized -  No model changed event
    //[JsonRequired]
    //// public List<Group> Groups { get; set; } = [];

    //[JsonIgnore]
    //// Not serialized -  With model changed event
    //// public Group? SelectedGroup { get => this.Get<Group?>(); set => this.Set(value); }

    //[JsonIgnore]
    //// Not serialized - No model changed event
    //// public List<string> AvailableIcons { get; set; } = [];

    public override async Task Initialize() => await this.Load();

    public override async Task Shutdown()
    {
        if (this.IsDirty)
        {
            await this.Save();
        }
    }

    public Task Load()
    {
        string filename = AstroPicModel.AstroPicModelFilename;
        try
        {
            if (!this.fileManager.Exists(Area.User, Kind.Json, filename))
            {
                this.fileManager.Save(Area.User, Kind.Json, filename, AstroPicModel.DefaultData);
            }

            AstroPicModel model =
                this.fileManager.Load<AstroPicModel>(Area.User, Kind.Json, filename);

            // Copy all properties with attribute [JsonRequired]
            base.CopyJSonRequiredProperties<AstroPicModel>(model);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            string msg = "Failed to load Model from " + filename;
            this.Logger.Fatal(msg);
            throw new Exception("", ex);
        }
    }

    public override Task Save()
    {
        // Null check is needed !
        // If the File Manager is null we are currently loading the model and activating properties on a second instance 
        // causing dirtyness, and in such case we must avoid the null crash and anyway there is no need to save anything.
        if (this.fileManager is not null)
        {
            this.fileManager.Save(Area.User, Kind.Json, AstroPicModel.AstroPicModelFilename, this);
            base.Save();
        }

        return Task.CompletedTask;
    }
}
