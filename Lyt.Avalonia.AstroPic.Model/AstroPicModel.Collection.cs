namespace Lyt.Avalonia.AstroPic.Model;

using static FileManagerModel;

public sealed partial class AstroPicModel : ModelBase
{
    private const int JpgMinLength = 256;
    private const int KB = 1024;
    private const int KBby2 = 512;

    // Size on disk is not portable, requires p/invoke or WMI on Windows 
    // therefore we only use an (very) raw estimate here with 100 KB per image
    // which seems to match what can be seen in the app image storage folder 
    private const long ClusteringAndFragmentationEstimatedSize = 20 * KB;

    private readonly Random random = new((int)DateTime.Now.Ticks);

    public string ProviderName(ProviderKey key)
    {
        string? name =
            (from provider in this.Providers 
             where provider.Key == key 
             select provider.Name)
             .FirstOrDefault();
        if (string.IsNullOrWhiteSpace(name))
        {
            this.Logger.Warning("Undefined Provider!");
            return "Unknown Provider";
        }

        return name;
    }

    public List<Tuple<Picture, byte[]>> LoadCollectionThumbnails()
    {
        List<Tuple<Picture, byte[]>> list = [];
        var pictures = this.Pictures.Values.ToList();
        foreach (var picture in pictures)
        {
            string name = picture.ThumbnailFilePath;
            if (this.ThumbnailCache.TryGetValue(name, out byte[]? bytes))
            {
                if (bytes is not null)
                {
                    list.Add(new Tuple<Picture, byte[]>(picture, bytes));
                }
            }
        }

        // Reorder the list, most recent first 
        var orderedList =
            (from picture in list
             orderby picture.Item1.PictureMetadata.Date descending
             select picture).ToList();
        return orderedList;
    }

    public bool AddToCollection(PictureMetadata pictureMetadata, byte[] imageBytes, byte[] thumbnailBytes)
    {
        // BUG ~ Problem ? 
        // If the service returns many images per day (Earth View) only one will be saved 
        // But there will be many entries in the dictionary 
        try
        {
            var picture = new Picture(pictureMetadata);
            if (pictureMetadata.Provider == ProviderKey.Unknown)
            {
                throw new Exception("No provider");
            }
            else if (pictureMetadata.Provider == ProviderKey.Personal)
            {
                // Update picture paths and URL 
                this.UpdatePersonalPictureData(picture);
            }
            else
            {
                // Web provider 
                string? url = pictureMetadata.Url;
                if (!string.IsNullOrWhiteSpace(url))
                {
                    if (this.Pictures.ContainsKey(url))
                    {
                        this.Logger.Warning(
                            "Picture already added to collection: " +
                            pictureMetadata.Provider.ToString());
                        // TODO: Messenger info 
                        return true;
                    }
                }
                else
                {
                    throw new Exception("Picture has no URL");
                }
            }

            if (imageBytes is not null &&
                imageBytes.Length > JpgMinLength &&
                thumbnailBytes is not null &&
                thumbnailBytes.Length > JpgMinLength &&
                !string.IsNullOrWhiteSpace(pictureMetadata.Url))
            {
                // Save images 
                this.fileManager.Save<byte[]>(
                    Area.User, Kind.BinaryNoExtension, picture.ImageFilePath, imageBytes);
                this.fileManager.Save<byte[]>(
                    Area.User, Kind.BinaryNoExtension, picture.ThumbnailFilePath, thumbnailBytes);

                // Add metadata 
                this.Pictures.Add(pictureMetadata.Url, picture);

                // Add thumbnail to cache, if needed
                bool added = this.ThumbnailCache.TryAdd(picture.ThumbnailFilePath, thumbnailBytes);
                if (!added)
                {
                    this.Logger.Warning("Failed to add picture thumbnail in cache");
                }

                // Save the whole model to disk and notify 
                this.Save();
                this.UpdateStatistics(isAdd: true);
                this.Messenger.Publish(new CollectionChangedMessage());

                return true;
            }
            else
            {
                throw new Exception("Missing image data");
            }
        }
        catch (Exception ex)
        {
            // TODO: Messenger warning 
            this.Logger.Warning(
                "Failed to add picture to collection" +
                pictureMetadata.Provider.ToString() + "\n" + ex.ToString());
            return false;
        }
    }

    public void RemoveFromCollection(PictureMetadata pictureMetadata)
    {
        try
        {
            string? url = pictureMetadata.Url;
            if (!string.IsNullOrWhiteSpace(url) && this.Pictures.ContainsKey(url))
            {
                if (this.Pictures.TryGetValue(url, out Picture? maybePicture))
                {
                    if (maybePicture is Picture picture)
                    {
                        // Delete images 
                        string imagePath = this.fileManager.MakePath(Area.User, Kind.BinaryNoExtension, picture.ImageFilePath);
                        File.Delete(imagePath);
                        string thumbnailPath = this.fileManager.MakePath(Area.User, Kind.BinaryNoExtension, picture.ThumbnailFilePath);
                        File.Delete(thumbnailPath);

                        // Remove metadata 
                        this.Pictures.Remove(url);

                        // Commit changes and notify
                        this.Save();
                        this.UpdateStatistics(isAdd: false);
                        this.Messenger.Publish(new CollectionChangedMessage(IsAddition: false));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // TODO: Messenger warning 
            this.Logger.Warning(
                "Failed to remove picture from collection" +
                pictureMetadata.Provider.ToString() + "\n" + ex.ToString());
        }
    }

    public void Update (PictureMetadata pictureMetadata)
    {
        try
        {
            string? url = pictureMetadata.Url;
            if (!string.IsNullOrWhiteSpace(url) && this.Pictures.ContainsKey(url))
            {
                if (this.Pictures.TryGetValue(url, out Picture? maybePicture))
                {
                    if (maybePicture is Picture picture)
                    {
                        // Update metadata 
                        picture.PictureMetadata = pictureMetadata;

                        // Commit changes
                        this.Save();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // TODO: Messenger warning 
            this.Logger.Warning(
                "Failed to remove picture from collection" +
                pictureMetadata.Provider.ToString() + "\n" + ex.ToString());
        }
    }

    public bool AreQuotasExceeded()
        => (this.Statistics.ImageCount > this.MaxImages) ||
           (this.Statistics.SizeOnDiskKB > this.MaxStorageMB * KB);

    public bool IsAvailableDiskSpaceLow()
    {
        long availableSpace = this.fileManager.AvailableFreeSpace(FileManagerModel.Area.User);
        if (availableSpace > 0)
        {
            double availableSpaceGB = availableSpace / (1024.0 * 1024.0 * 1024.0);
            return availableSpaceGB < 2.0;
        }
        else
        {
            return true;
        }
    }

    public void CleanupCollection(bool calledFromUi = false)
    {
        if (!calledFromUi && !this.ShouldAutoCleanup)
        {
            return;
        }

        if (this.AreQuotasExceeded())
        {
            this.DoCleanup();
            this.ValidateCollection();
            this.Messenger.Publish(new CollectionChangedMessage(IsAddition: false));
        }
    }

    private void ValidateCollection()
    {
        const long imageMinSize = 64 * KB;
        const long thumbnailMinSize = 2 * KB;

        int validImageCount = 0;
        long sizeOnDisk = 0;
        List<Picture> picturesToRemove = [];
        foreach (var picture in this.Pictures.Values)
        {
            bool isOk = false;
            string imagePath = this.fileManager.MakePath(Area.User, Kind.BinaryNoExtension, picture.ImageFilePath);
            var imageFileInfo = new FileInfo(imagePath);
            string thumbnailPath = this.fileManager.MakePath(Area.User, Kind.BinaryNoExtension, picture.ThumbnailFilePath);
            var thumbnailFileInfo = new FileInfo(thumbnailPath);
            if (imageFileInfo.Exists && thumbnailFileInfo.Exists)
            {
                long imageSize = imageFileInfo.Length;
                long thumbnailSize = thumbnailFileInfo.Length;
                if ((imageSize > imageMinSize) && (thumbnailSize > thumbnailMinSize))
                {
                    ++validImageCount;
                    sizeOnDisk += imageSize + 4 * ClusteringAndFragmentationEstimatedSize;
                    sizeOnDisk += thumbnailSize + ClusteringAndFragmentationEstimatedSize;
                    isOk = true;
                }
            }

            if (!isOk)
            {
                picturesToRemove.Add(picture);
            }
        }

        // Silently remove from the collection all images that are missing, most likely that have been
        // deleted or moved.
        foreach (var picture in picturesToRemove)
        {
            string? url = picture.PictureMetadata.Url;
            if (!string.IsNullOrWhiteSpace(url))
            {
                this.Pictures.Remove(url);
            }
        }

        this.Statistics =
            new Statistics(
                ImageCount: validImageCount,
                SizeOnDiskKB: (int)((512 + sizeOnDisk) / KB));
    }

    private void DoCleanup()
    {
        // Order the pictures, oldest first, copy list 
        List<Picture> orderedList =
            [.. (from picture in this.Pictures.Values
             orderby picture.PictureMetadata.Date ascending
             select picture)];

        long deletedSizeOnDisk = 0;
        int countToDelete = this.Statistics.ImageCount - this.MaxImages;
        if (countToDelete > 0)
        {
            for (int i = 0; i < countToDelete; ++i)
            {
                if (i < orderedList.Count)
                {
                    var picture = orderedList[i];
                    deletedSizeOnDisk += this.RemovePicture(picture);
                }
            }
        }

        // Update statistics on delete 
        this.UpdateStatisticsOnDelete(deletedSizeOnDisk);

        // Re-Order the pictures, oldest first, copy again the list to make sure we are 
        // NOT going to process twice the same entries 
        orderedList =
            [.. (from picture in this.Pictures.Values
             orderby picture.PictureMetadata.Date ascending
             select picture)];
        int deleteIndex = 0;
        countToDelete = Math.Min(5, orderedList.Count);
        while (countToDelete > 0)
        {
            if (this.Statistics.SizeOnDiskKB < this.MaxStorageMB * KB)
            {
                break;
            }

            var picture = orderedList[deleteIndex];
            deletedSizeOnDisk = this.RemovePicture(picture);
            this.UpdateStatisticsOnDelete(deletedSizeOnDisk);

            ++deleteIndex;
            --countToDelete;
        }
    }

    private long RemovePicture(Picture picture)
    {
        long sizeOnDisk = 0;
        string imagePath = this.fileManager.MakePath(Area.User, Kind.BinaryNoExtension, picture.ImageFilePath);
        var imageFileInfo = new FileInfo(imagePath);
        string thumbnailPath = this.fileManager.MakePath(Area.User, Kind.BinaryNoExtension, picture.ThumbnailFilePath);
        var thumbnailFileInfo = new FileInfo(thumbnailPath);
        if (imageFileInfo.Exists && thumbnailFileInfo.Exists)
        {
            long imageSize = imageFileInfo.Length;
            long thumbnailSize = thumbnailFileInfo.Length;
            sizeOnDisk += imageSize + 4 * ClusteringAndFragmentationEstimatedSize;
            sizeOnDisk += thumbnailSize + ClusteringAndFragmentationEstimatedSize;

            File.Delete(imagePath);
            File.Delete(thumbnailPath);
            if (!string.IsNullOrWhiteSpace(picture.PictureMetadata.Url))
            {
                this.Pictures.Remove(picture.PictureMetadata.Url);
            }
        }

        return sizeOnDisk;
    }

    private void UpdatePersonalPictureData(Picture picture)
    {
        static int ParsePath(string path)
        {
            path = path.Replace("Personal_", string.Empty);
            string index = path[..6];
            return int.Parse(index);
        }

        var personalPicturesIndices =
            (from pic in this.Pictures.Values
             where pic.PictureMetadata.Provider == ProviderKey.Personal
             select ParsePath(pic.ImageFilePath)).ToList();
        int index = 0;
        if (personalPicturesIndices.Count == 0)
        {
            index = 1;
        }
        else
        {
            index = 1 + personalPicturesIndices.Max();
        }

        // extension includes the dot
        string extension = Path.GetExtension(picture.ImageFilePath);
        picture.ImageFilePath = string.Format("Personal_{0:D6}{1}", index, extension);
        picture.ThumbnailFilePath = string.Format("Personal_{0:D6}_Thumb{1}", index, extension);
    }

    private void UpdateStatisticsOnDelete(long deletedSizeOnDisk)
    {
        long newSizeOnDisk = this.Statistics.SizeOnDiskKB * KB - deletedSizeOnDisk;
        this.Statistics =
            new Statistics(
                ImageCount: this.Pictures.Count,
                SizeOnDiskKB: (int)((KBby2 + newSizeOnDisk) / KB));
    }

    private void UpdateStatistics(bool isAdd)
    {
        // Yes, we are cheating for the sake of being fast and efficient
        int deltaKB = isAdd ? 333 : -299;
        int sizeOnDisk = (this.Statistics.SizeOnDiskKB + deltaKB) * KB;
        this.Statistics =
            new Statistics(
                ImageCount: this.Pictures.Count,
                SizeOnDiskKB: (int)((KBby2 + sizeOnDisk) / KB));
    }

    private void LoadThumbnailCache()
    {
        this.ThumbnailCache.Clear();
        var pictures = this.Pictures.Values.ToList();

        // Speed up the loading of the collection using threads (aka tasks) 
        void LoadPicture(int from, int to)
        {
            for (int k = from; k < to; ++k)
            {
                var picture = pictures[k];
                string name = picture.ThumbnailFilePath;
                var fileId = new FileId(Area.User, Kind.BinaryNoExtension, name);
                try
                {
                    if (this.fileManager.Exists(fileId))
                    {
                        byte[] bytes = this.fileManager.Load<byte[]>(fileId);
                        lock (this.ThumbnailCache)
                        {
                            this.ThumbnailCache.Add(name, bytes);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
            }
        }

        // 1 : Setup
        int count = 4; // Consider: Environment.ProcessorCount;  
        int all = pictures.Count;
        int half = all / 2;
        int quart = half / 2;

        // Consider improving: a bit suboptimal if all is odd...
        int[] indices = [0, quart, half, half + quart, all];
        var tasks = new Task[count];
        for (int taskIndex = 0; taskIndex < count; ++taskIndex)
        {
            int from = indices[taskIndex];
            int to = indices[1 + taskIndex];
            Debug.WriteLine("From: " + from + " To: " + to);
            var task = new Task(() => LoadPicture(from, to));
            tasks[taskIndex] = task;
        }

        // 2 : Start all tasks
        for (int taskIndex = 0; taskIndex < count; ++taskIndex)
        {
            tasks[taskIndex].Start();
        }

        // 3 : Wait for completion 
        Task.WaitAll(tasks);

        this.ThumbnailsLoaded = true;
        this.NotifyModelLoaded();
    }

    private void NotifyModelLoaded()
    {
        lock (this.lockObject)
        {
            if (!this.ModelLoadedNotified && this.ThumbnailsLoaded && this.PingComplete)
            {
                this.Messenger.Publish(new ModelLoadedMessage());
                this.ModelLoadedNotified = true;
            }
        }
    }
}