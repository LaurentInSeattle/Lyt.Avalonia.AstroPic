namespace Lyt.Avalonia.AstroPic.Workflow.Collection;

public sealed class StatisticsViewModel : Bindable<StatisticsView>
{
    private readonly AstroPicModel astroPicModel; 

    public StatisticsViewModel(AstroPicModel astroPicModel)
    {
        this.astroPicModel = astroPicModel;
        this.Messenger.Subscribe<ModelLoadedMessage>(this.OnModelLoaded);
        this.Messenger.Subscribe<CollectionChangedMessage>(this.OnCollectionChanged);
    }

    private void OnModelLoaded(ModelLoadedMessage _)
            => this.UpdateStatistics();

    private void OnCollectionChanged(CollectionChangedMessage message)
            => this.UpdateStatistics();

    private void UpdateStatistics()
    {
        var statistics = this.astroPicModel.Statistics;
        this.ImageCountText =
            string.Format("Immagine: {0} (Tutti i servizi - Quota: {1})", statistics.ImageCount, this.astroPicModel.MaxImages);
        int sizeOnDisk = (int)((statistics.SizeOnDiskKB + 512 + 1) / 1024);
        this.SizeOnDiskText =
            string.Format("Dimensione stimata su disco: {0} MB (Quota: {1} MB)", sizeOnDisk, this.astroPicModel.MaxStorageMB);
        var fileManager = App.GetRequiredService<FileManagerModel>();
        long availableSpace = fileManager.AvailableFreeSpace(FileManagerModel.Area.User);
        if (availableSpace > 0)
        {
            double availableSpaceGB = availableSpace / (1024.0 * 1024.0 * 1024.0);
            this.AvailableDiskSpaceText =
                string.Format("Spazio disponibile su disco: {0:F1} GB", availableSpaceGB);
        }
        else
        {
            // Could not figure out drive name ? 
            this.AvailableDiskSpaceText = string.Empty;
        }

        this.AlertText = string.Empty;
        if ( this.astroPicModel.IsAvailableDiskSpaceLow())
        {
            this.AlertText = "Attento al spazio disponibile su disco!";
        }
        else
        {
            if (this.astroPicModel.AreQuotasExceeded())
            {
                this.AlertText = "Troppe immagini nella collezione!";
            }
        } 
    }

    public string ImageCountText { get => this.Get<string>()!; set => this.Set(value); }

    public string SizeOnDiskText { get => this.Get<string>()!; set => this.Set(value); }

    public string AvailableDiskSpaceText { get => this.Get<string>()!; set => this.Set(value); }

    public string AlertText { get => this.Get<string>()!; set => this.Set(value); }
}
