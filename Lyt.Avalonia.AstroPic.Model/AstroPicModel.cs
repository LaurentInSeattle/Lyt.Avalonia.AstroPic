namespace Lyt.Avalonia.AstroPic.Model;

using static Lyt.Avalonia.Persistence.FileManagerModel;

public sealed partial class AstroPicModel : ModelBase
{
    public const string DefaultLanguage = "it-IT";
    private const string AstroPicModelFilename = "AstroPicData";

    private static readonly AstroPicModel DefaultData =
        new()
        {
            Language = DefaultLanguage,
            IsFirstRun = true,
            ShouldAutoStart = false,
            MaxImages = 128,
            MaxStorageMB = 64,
            Providers =
            [
                new Provider(ProviderKey.Nasa),
                new Provider(ProviderKey.Bing),
                new Provider(ProviderKey.EarthView),
            ]
        };

    private readonly FileManagerModel fileManager;
    private readonly AstroPicService astroPicService;
    private readonly IWallpaperService wallpaperService;

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

    public AstroPicModel(
        FileManagerModel fileManager, 
        AstroPicService astroPicService,
        IWallpaperService wallpaperService,
        IMessenger messenger, ILogger logger) : base(messenger, logger)
    {
        this.fileManager = fileManager;
        this.astroPicService = astroPicService;
        this.wallpaperService = wallpaperService;
        this.ShouldAutoSave = true;
    }

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
