namespace Lyt.Avalonia.AstroPic.Model;

using Lyt.Avalonia.AstroPic.Model.DataObjects;
using System;
using static FileManagerModel;

public sealed partial class AstroPicModel : ModelBase
{
    private const int JpgMinLength = 256;

    private readonly Random random = new((int)DateTime.Now.Ticks);

    public string ProviderName(ProviderKey key)
    {
        string? name =
            (from provider in this.Providers where provider.Key == key select provider.Name).FirstOrDefault();
        if (string.IsNullOrWhiteSpace(name))
        {
            this.Logger.Warning("Undefined Provider!");
            return "Unknown Provider";
        }

        return name;
    }

    public void SetWallpaper(PictureMetadata pictureMetadata, byte[] imageBytes)
    {
        ProviderKey provider = pictureMetadata.Provider;
        try
        {
            string name = provider.ToString();
            var fileId = new FileId(Area.User, Kind.BinaryNoExtension, name);
            if (this.fileManager.Exists(fileId))
            {
                this.fileManager.Delete(fileId);
            }

            string path = this.fileManager.MakePath(fileId);
            this.fileManager.Save<byte[]>(fileId, imageBytes);
            this.SetWallpaper(path);
            this.fileManager.Delete(fileId);
        }
        catch (Exception ex)
        {
            this.Logger.Warning(
                "Failed to set wallpaper " + provider.ToString() + "\n" + ex.ToString());
        }
    }

    public void RotateWallpaper()
    {
        if (this.Pictures.Values.Count == 0)
        {
            // No data, most likely first run
            return;
        }

        // Pickup a new wallpaper not present in the MRU list
        if (this.MruWallpapers.Count == this.Pictures.Values.Count)
        {
            this.Logger.Info("MRU Wallpapers cleared");
            this.MruWallpapers.Clear();
        }

        List<string> files = [];
        foreach (var picture in this.Pictures.Values)
        {
            string name = picture.ImageFilePath;
            string path = this.fileManager.MakePath(Area.User, Kind.BinaryNoExtension, name);
            if (this.MruWallpapers.Contains(path))
            {
                continue;
            }

            files.Add(path);
        }

        if (files.Count == 1)
        {
            this.MruWallpapers.Clear();
            this.Logger.Info("MRU Wallpapers cleared");
            this.SetWallpaper(files[0]);
        }
        else
        {
            int index = this.random.Next(files.Count);
            this.SetWallpaper(files[index]);
        }
    }

    private void SetWallpaper(string path)
    {
        try
        {
            if (!Path.Exists(path))
            {
                throw new Exception("Does not exists.");
            }

            this.MruWallpapers.Add(path);
            this.wallpaperService.Set(path, WallpaperStyle.Fill);
            this.Logger.Info("Wallpaper set: " + path);
        }
        catch (Exception ex)
        {
            this.Logger.Warning(
                "Failed to set wallpaper: " + path + "\n" + ex.ToString());
        }
    }

    public bool IsUpdatingTodayImagesNeeded()
    {
        foreach (var provider in this.Providers)
        {
            // Use ONLY enabled and selected providers 
            if (!provider.IsSelected)
            {
                continue;
            }

            if (!provider.IsLoaded)
            {
                return true;
            }
        }

        return false;
    }

    public async Task<List<PictureDownload>> DownloadTodayImages()
    {
        // Check Internet is done elsewhere, here it is assumed we are connected 

        var downloads = new List<PictureDownload>(this.Providers.Count);
        foreach (var provider in this.Providers)
        {
            // Use ONLY enabled and selected providers 
            if (!provider.IsSelected)
            {
                continue;
            }

            // Check if we already have the data in local storage
            provider.IsLoaded = false;
            if (this.LastUpdate.TryGetValue(provider.Key, out PictureMetadata? maybePictureMetadata))
            {
                if ((maybePictureMetadata is PictureMetadata pictureMetadata) &&
                    (pictureMetadata.Date == DateTime.Now.Date))
                {
                    try
                    {
                        // load image from disk
                        byte[] imageBytes = this.fileManager.Load<byte[]>(
                            Area.User, Kind.BinaryNoExtension, pictureMetadata.TodayImageFilePath());
                        downloads.Add(new PictureDownload(pictureMetadata, imageBytes));
                        provider.IsLoaded = true;
                    }
                    catch (Exception e1)
                    {
                        this.Logger.Warning(
                            "Failed to load image: " + "\n" + e1.ToString());
                    }
                } // else we will download it 
            }
        }

        bool needToSaveModel = false;
        foreach (var provider in this.Providers)
        {
            // Use ONLY enabled and selected providers 
            if (!provider.IsSelected)
            {
                continue;
            }

            // Skip images already loaded 
            if (provider.IsLoaded)
            {
                continue;
            }

            // We dont want to download everything in parallel so that we do not
            // overwhelm the computer Internet bandwith 
            if (downloads.Count > 1)
            {
                // A little pause between starting the next provider 
                await Task.Delay(100);
            }


            try
            {
                var download = await this.DownloadImage(provider.Key);
                if (download.IsValid)
                {
                    downloads.Add(download);

                    // Save metadata and image bytes on disk  
                    try
                    {
                        var meta = download.PictureMetadata;
                        this.fileManager.Save<byte[]>(
                            Area.User, Kind.BinaryNoExtension,
                            meta.TodayImageFilePath(), download.ImageBytes);

                        if (this.LastUpdate.TryGetValue(provider.Key, out _))
                        {
                            this.LastUpdate[provider.Key] = meta;
                        }
                        else
                        {
                            this.LastUpdate.Add(provider.Key, meta);
                        }

                        needToSaveModel = true;
                    }
                    catch (Exception e2)
                    {
                        this.Logger.Warning(
                            "Failed to save image: " + "\n" + e2.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                this.Logger.Warning(
                    "Failed to load picture from " + provider.ToString() + "\n" + ex.ToString());
            }
        }

        if (needToSaveModel)
        {
            await this.Save();

            // If we need to save the model, we have downloaded new stuff 
            // so we clear the most recently used wallpapers
            this.MruWallpapers.Clear();
        }

        return downloads;
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

                // Save the whole model to disk 
                this.Save();
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
            if (!string.IsNullOrWhiteSpace(url) &&
                this.Pictures.ContainsKey(url))
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

    private async Task<PictureDownload> DownloadImage(ProviderKey provider)
    {
        var empty = new PictureDownload(new(), []);
        try
        {
            this.Report(provider, isMetadata: true, isBegin: true);
            var metadata = await this.astroPicService.GetPictures(provider);
            if ((metadata != null) && (metadata.Count > 0))
            {
                this.Report(provider, isMetadata: true, isBegin: false);
                var picture = metadata[0];
                if (picture.MediaType == MediaType.Image)
                {
                    if (!string.IsNullOrWhiteSpace(picture.Url))
                    {
                        if (!this.IsAlreadyInCollection(picture))
                        {
                            // A little pause so that we can see the messaging 
                            await Task.Delay(100);
                            this.Report(provider, isMetadata: false, isBegin: true);

                            // A little pause between starting the actual image download 
                            await Task.Delay(100);
                            byte[] bytes = await this.astroPicService.DownloadPicture(picture);
                            this.Report(provider, isMetadata: false, isBegin: false);
                            return new PictureDownload(picture, bytes);
                        }
                        else
                        {
                            throw new Exception("Picture already downloaded.");
                        }
                    }
                    else
                    {
                        throw new Exception("Picture metadata has no URL.");
                    }
                }
                else
                {
                    throw new Exception("Picture is not an image.");
                }
            }
            else
            {
                // Nasa Astronomy Picture of the Day can be a video 
                string msg = "The Picture of the Day is actually a video clip.";
                this.ReportError(provider, msg);
                throw new Exception("Failed to retrieve picture metadata.");
            }
        }
        catch (Exception ex)
        {
            this.ReportError(provider, ex.Message);
            this.Logger.Warning(
                "Failed to load picture from " + provider.ToString() + "\n" +
                ex.Message + "\n" + ex.ToString());
        }

        return empty;
    }

    private void ReportError(ProviderKey provider, string message)
        => this.Messenger.Publish(new ServiceErrorMessage(provider, message));

    private void Report(ProviderKey provider, bool isMetadata, bool isBegin)
        => this.Messenger.Publish(
            new ServiceProgressMessage(provider, IsMetadata: isMetadata, IsBegin: isBegin));

    private bool IsAlreadyInCollection(PictureMetadata picture)
        => !string.IsNullOrWhiteSpace(picture.Url) &&
            this.Pictures.ContainsKey(picture.Url);

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

    public void LoadThumbnailCache()
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