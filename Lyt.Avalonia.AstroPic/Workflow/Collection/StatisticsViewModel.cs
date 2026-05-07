namespace Lyt.Avalonia.AstroPic.Workflow.Collection;

public sealed partial class StatisticsViewModel : 
    ViewModel<StatisticsView>,
    IRecipient<ModelLoadedMessage>,
    IRecipient<CollectionChangedMessage>,
    IRecipient<LanguageChangedMessage>
{
    private const double GigaByte = 1024.0 * 1024.0 * 1024.0; 

    private readonly AstroPicModel astroPicModel;

    [ObservableProperty]
    public partial string ImageCountText { get; set; }

    [ObservableProperty]
    public partial string SizeOnDiskText { get; set; }

    [ObservableProperty]
    public partial string AvailableDiskSpaceText { get; set; }

    [ObservableProperty]
    public partial string AlertText { get; set; }

    public StatisticsViewModel(AstroPicModel astroPicModel)
    {
        this.astroPicModel = astroPicModel;
        this.AlertText = string.Empty;
        this.ImageCountText = string.Empty;
        this.SizeOnDiskText = string.Empty;
        this.AvailableDiskSpaceText = string.Empty;
        this.Subscribe<ModelLoadedMessage>();
        this.Subscribe<CollectionChangedMessage>();
        this.Subscribe<LanguageChangedMessage>();
    }

    public void Receive(ModelLoadedMessage  _) => this.UpdateStatistics();

    public void Receive(CollectionChangedMessage _) => this.UpdateStatistics();

    public void Receive(LanguageChangedMessage _) => this.UpdateStatistics();

    private void UpdateStatistics()
    {
        var statistics = this.astroPicModel.Statistics;
        string formatImageCount = this.Localize("Collection.Stats.ImageCountFormat");
        this.ImageCountText =
            string.Format(formatImageCount, statistics.ImageCount, this.astroPicModel.MaxImages);
        int sizeOnDisk = (int)((statistics.SizeOnDiskKB + 512 + 1) / 1024);
        string formatSizeOnDisk = this.Localize("Collection.Stats.SizeOnDiskFormat");        
        this.SizeOnDiskText =
            string.Format(formatSizeOnDisk, sizeOnDisk, this.astroPicModel.MaxStorageMB);
        var fileManager = App.GetRequiredService<FileManagerModel>();
        long availableSpace = fileManager.AvailableFreeSpace(FileManagerModel.Area.User);
        if (availableSpace > 0)
        {
            double availableSpaceGB = availableSpace / GigaByte;
            string formatSpace = this.Localize("Collection.Stats.AvailableDiskSpaceFormat"); 
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
            this.AlertText = this.Localize("Collection.Stats.AlertTextSpace");
        }
        else
        {
            if (this.astroPicModel.AreQuotasExceeded())
            {
                this.AlertText = this.Localize("Collection.Stats.AlertTextQuota"); 
            }
        } 
    }
}
