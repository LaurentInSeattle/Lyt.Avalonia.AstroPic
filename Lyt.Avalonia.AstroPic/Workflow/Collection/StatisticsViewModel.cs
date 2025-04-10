namespace Lyt.Avalonia.AstroPic.Workflow.Collection;

public sealed class StatisticsViewModel : Bindable<StatisticsView>
{
    public StatisticsViewModel()
    {
        this.Messenger.Subscribe<ModelLoadedMessage>(this.OnModelLoaded);
        this.Messenger.Subscribe<CollectionChangedMessage>(this.OnCollectionChanged);
    }

    private void OnModelLoaded(ModelLoadedMessage _)
            => this.UpdateStatistics();

    private void OnCollectionChanged(CollectionChangedMessage message)
            => this.UpdateStatistics();

    private void UpdateStatistics()
    {
        var astroPicModel = App.GetRequiredService<AstroPicModel>();
        var statistics = astroPicModel.Statistics;
        this.ImageCountText =
            string.Format("Immagine: {0} (Tutti i servizi - Quota: {1})", statistics.ImageCount, astroPicModel.MaxImages);
        int sizeOnDisk = (int)((statistics.SizeOnDiskKB + 512 + 1) / 1024);
        this.SizeOnDiskText =
            string.Format("Dimensione stimata su disco: {0} MB (Quota: {1} MB)", sizeOnDisk, astroPicModel.MaxStorageMB);
        var filemanager = App.GetRequiredService<FileManagerModel>();
        long availableSpace = filemanager.AvailableFreeSpace(FileManagerModel.Area.User);
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
    }

    public string ImageCountText { get => this.Get<string>()!; set => this.Set(value); }

    public string SizeOnDiskText { get => this.Get<string>()!; set => this.Set(value); }

    public string AvailableDiskSpaceText { get => this.Get<string>()!; set => this.Set(value); }
}
