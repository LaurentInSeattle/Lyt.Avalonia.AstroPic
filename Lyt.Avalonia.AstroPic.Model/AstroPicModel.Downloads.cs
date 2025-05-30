namespace Lyt.Avalonia.AstroPic.Model;

using static FileManagerModel;

public sealed partial class AstroPicModel : ModelBase
{
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
            if (! provider.IsDownloadProvider || !provider.IsSelected)
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

    private async Task<PictureDownload> DownloadImage(ImageProviderKey provider)
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
                            // Picture already downloaded.
                            throw new Exception("Model.AlreadyDownloaded");
                        }
                    }
                    else
                    {
                        // Picture metadata has no URL.
                        throw new Exception("Model.NoUrl");                        
                    }
                }
                else
                {
                    // Picture is not a valid image.
                    throw new Exception("Model.NotAnImage");
                }
            }
            else
            {
                // Nasa Astronomy Picture of the Day can be a video 
                // The Picture of the Day is actually a video clip.
                string msg = "Model.TodayIsVideo";
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

    private void ReportError(ImageProviderKey provider, string message)
        => this.Messenger.Publish(new ServiceErrorMessage(provider, message));

    private void Report(ImageProviderKey provider, bool isMetadata, bool isBegin)
        => this.Messenger.Publish(
            new ServiceProgressMessage(provider, IsMetadata: isMetadata, IsBegin: isBegin));

    private bool IsAlreadyInCollection(PictureMetadata picture)
        => !string.IsNullOrWhiteSpace(picture.Url) &&
            this.Pictures.ContainsKey(picture.Url);
}
