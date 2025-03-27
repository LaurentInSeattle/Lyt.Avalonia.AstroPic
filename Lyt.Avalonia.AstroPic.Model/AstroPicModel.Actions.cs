namespace Lyt.Avalonia.AstroPic.Model;

public sealed partial class AstroPicModel : ModelBase
{
    public async Task<List<PictureDownload>> DownloadTodayImages()
    {
        // TODO: Check Internet: See what we did for Cranky 


        Provider[] providers = [Provider.Nasa, Provider.Bing];
        var downloads = new List<PictureDownload>(providers.Length); 
        foreach (var provider in providers)
        {
            // We dont want to download everything in paralel so that we do not
            // overwhelm the computer Internet bandwith 
            if (downloads.Count > 1)
            {
                // A little pause between starting the next provider 
                await Task.Delay(500);
            } 

            var download = await this.DownloadImage(provider);
            if (download.IsValid)
            {
                downloads.Add(download);
                // TODO: send progress message 
            } 
        } 

        return downloads;
    }

    private async Task<PictureDownload> DownloadImage(Provider provider)
    {
        var empty = new PictureDownload(new(), []);
        try
        {
            var metadata = await AstroPicService.GetPictures(provider, DateTime.Now);
            if ((metadata != null) && (metadata.Count > 0))
            {
                var picture = metadata[0];
                if (picture.MediaType == MediaType.Image)
                {
                    if (!string.IsNullOrWhiteSpace(picture.Url))
                    {
                        if (!this.IsAlreadyInCollection(picture))
                        {
                            byte[] bytes = await AstroPicService.DownloadPicture(picture);
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
            this.Logger.Warning(
                "Failed to load picture from " + provider.ToString() + "\n" + ex.ToString());
        }

        return empty;
    }

    private bool IsAlreadyInCollection (PictureMetadata picture)
        => !string.IsNullOrWhiteSpace(picture.Url) && 
            this.Collection.Pictures.ContainsKey(picture.Url);
}
