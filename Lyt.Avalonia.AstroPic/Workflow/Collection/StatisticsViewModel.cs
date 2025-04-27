namespace Lyt.Avalonia.AstroPic.Workflow.Collection;

public sealed class StatisticsViewModel : Bindable<StatisticsView>
{
    private readonly AstroPicModel astroPicModel; 

    public StatisticsViewModel(AstroPicModel astroPicModel)
    {
        this.astroPicModel = astroPicModel;
        this.Messenger.Subscribe<ModelLoadedMessage>(this.OnModelLoaded);
        this.Messenger.Subscribe<CollectionChangedMessage>(this.OnCollectionChanged);
        this.Messenger.Subscribe<LanguageChangedMessage>(this.OnLanguageChanged);
    }

    private void OnModelLoaded(ModelLoadedMessage _) => this.UpdateStatistics();

    private void OnCollectionChanged(CollectionChangedMessage message) => this.UpdateStatistics();

    private void OnLanguageChanged(LanguageChangedMessage message) => this.UpdateStatistics();

    private void UpdateStatistics()
    {
        var statistics = this.astroPicModel.Statistics;
        string formatImageCount = this.Localizer.Lookup("Collection.Stats.ImageCountFormat");
        this.ImageCountText =
            string.Format(formatImageCount, statistics.ImageCount, this.astroPicModel.MaxImages);
        int sizeOnDisk = (int)((statistics.SizeOnDiskKB + 512 + 1) / 1024);
        string formatSizeOnDisk = this.Localizer.Lookup("Collection.Stats.SizeOnDiskFormat");        
        this.SizeOnDiskText =
            string.Format(formatSizeOnDisk, sizeOnDisk, this.astroPicModel.MaxStorageMB);
        var fileManager = App.GetRequiredService<FileManagerModel>();
        long availableSpace = fileManager.AvailableFreeSpace(FileManagerModel.Area.User);
        if (availableSpace > 0)
        {
            double availableSpaceGB = availableSpace / (1024.0 * 1024.0 * 1024.0);
            string formatSpace = this.Localizer.Lookup("Collection.Stats.AvailableDiskSpaceFormat"); 
            this.AvailableDiskSpaceText =string.Format(formatSpace, availableSpaceGB);
        }
        else
        {
            // Could not figure out drive name ? 
            this.AvailableDiskSpaceText = string.Empty;
        }

        this.AlertText = string.Empty;
        if ( this.astroPicModel.IsAvailableDiskSpaceLow())
        {
            // "Attento al spazio disponibile su disco!";
            this.AlertText = this.Localizer.Lookup("Collection.Stats.AlertTextSpace");
        }
        else
        {
            if (this.astroPicModel.AreQuotasExceeded())
            {
                // "Troppe immagini nella collezione!";
                this.AlertText = this.Localizer.Lookup("Collection.Stats.AlertTextQuota"); 
            }
        } 
    }

    public string ImageCountText { get => this.Get<string>()!; set => this.Set(value); }

    public string SizeOnDiskText { get => this.Get<string>()!; set => this.Set(value); }

    public string AvailableDiskSpaceText { get => this.Get<string>()!; set => this.Set(value); }

    public string AlertText { get => this.Get<string>()!; set => this.Set(value); }
}
