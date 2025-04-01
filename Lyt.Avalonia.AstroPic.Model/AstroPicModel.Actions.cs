namespace Lyt.Avalonia.AstroPic.Model;

using static FileManagerModel;

public sealed partial class AstroPicModel : ModelBase
{
    private const int JpgMinLength = 256;

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

    public void SetWallpaper(PictureDownload download)
    {
        ProviderKey provider = download.PictureMetadata.Provider;
        try
        {
            string name = provider.ToString();
            var fileId = new FileId(Area.User, Kind.BinaryNoExtension, name);
            if (this.fileManager.Exists(fileId))
            {
                this.fileManager.Delete(fileId);
            }

            string path = this.fileManager.MakePath(fileId);
            this.fileManager.Save<byte[]>(fileId, download.ImageBytes);
            this.wallpaperService.Set(path, WallpaperStyle.Fill);
            this.fileManager.Delete(fileId);
        }
        catch (Exception ex)
        {
            this.Logger.Warning(
                "Failed to set wallpaper " + provider.ToString() + "\n" + ex.ToString());
        }
    }

    public async Task<List<PictureDownload>> DownloadTodayImages()
    {
        // TODO: Check Internet: See what we did for Cranky 
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
                    catch (Exception e1 )
                    { 
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
                        if (this.LastUpdate.TryGetValue(provider.Key, out _))
                        {
                            this.LastUpdate[provider.Key] = meta;
                        }
                        else
                        {
                            this.LastUpdate.Add(provider.Key, meta);
                        }

                        this.fileManager.Save<byte[]>(
                            Area.User, Kind.BinaryNoExtension,
                            meta.TodayImageFilePath(), download.ImageBytes);

                        needToSaveModel = true;
                    }
                    catch (Exception e2)
                    {
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
        }

        return downloads;
    }

    public bool AddToCollection(PictureDownload download)
    {
        // BUG ~ Problem ? 
        // If the service returns many images per day (Earth View) only one will be saved 
        // But there will be many entries in the dictionary 
        try
        {
            var metadata = download.PictureMetadata;
            string? url = metadata.Url;
            if (!string.IsNullOrWhiteSpace(url))
            {
                if (this.Pictures.ContainsKey(url))
                {
                    this.Logger.Warning(
                        "Picture already added to collection: " +
                        download.PictureMetadata.Provider.ToString());
                    // TODO: Messenger info 
                    return true;
                }
            }
            else
            {
                throw new Exception("Picture has no URL");
            }

            var picture = new Picture(metadata);
            byte[] imageBytes = download.ImageBytes;
            byte[]? thumbnailBytes = download.ThumbnailBytes;
            if (imageBytes is not null &&
                imageBytes.Length > JpgMinLength &&
                thumbnailBytes is not null &&
                thumbnailBytes.Length > JpgMinLength)
            {
                // Save images 
                this.fileManager.Save<byte[]>(
                    Area.User, Kind.BinaryNoExtension, picture.ImageFilePath, imageBytes);
                this.fileManager.Save<byte[]>(
                    Area.User, Kind.BinaryNoExtension, picture.ThumbnailFilePath, thumbnailBytes);

                // Save metadata 
                this.Pictures.Add(url, picture);
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
                download.PictureMetadata.Provider.ToString() + "\n" + ex.ToString());
            return false;
        }
    }

    private async Task<PictureDownload> DownloadImage(ProviderKey provider)
    {
        var empty = new PictureDownload(new(), []);
        try
        {
            this.Report(provider, isMetadata: true, isBegin: true);
            var metadata = await this.astroPicService.GetPictures(provider, DateTime.Now);
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
}
