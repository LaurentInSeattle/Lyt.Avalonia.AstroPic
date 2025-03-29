namespace Lyt.Avalonia.AstroPic.Model;

public sealed partial class AstroPicModel : ModelBase
{
    public async Task<List<PictureDownload>> DownloadTodayImages()
    {
        // TODO: Check Internet: See what we did for Cranky 

        // TODO: Use ONLY enabled and selected providers 
        Provider[] providers = [Provider.Nasa, Provider.Bing, Provider.EarthView];

        var downloads = new List<PictureDownload>(providers.Length); 
        foreach (var provider in providers)
        {
            // We dont want to download everything in paralel so that we do not
            // overwhelm the computer Internet bandwith 
            if (downloads.Count > 1)
            {
                // A little pause between starting the next provider 
                await Task.Delay(100);
            }

            try
            {
                var download = await this.DownloadImage(provider);
                if (download.IsValid)
                {
                    downloads.Add(download);
                }
            }
            catch (Exception ex)
            {
                this.Logger.Warning(
                    "Failed to load picture from " + provider.ToString() + "\n" + ex.ToString());
            }
        } 

        return downloads;
    }

    private async Task<PictureDownload> DownloadImage(Provider provider)
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

    private void ReportError(Provider provider, string message)
        => this.Messenger.Publish(new ServiceErrorMessage(provider, message));

    private void Report (Provider provider, bool isMetadata, bool isBegin)
        => this.Messenger.Publish(
            new ServiceProgressMessage(provider, IsMetadata: isMetadata, IsBegin: isBegin));

    private bool IsAlreadyInCollection (PictureMetadata picture)
        => !string.IsNullOrWhiteSpace(picture.Url) && 
            this.Collection.Pictures.ContainsKey(picture.Url);
}
