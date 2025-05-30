namespace Lyt.Avalonia.AstroPic.Service;

public class AstroPicService(ILogger logger, IRandomizer randomizer)
{
    private readonly ILogger logger = logger;
    private readonly IRandomizer randomizer = randomizer;

    public async Task<List<PictureMetadata>> GetPictures(ImageProviderKey provider)
    {
        try
        {
            switch (provider)
            {
                case ImageProviderKey.Nasa:
                    return await NasaService.GetPictures();

                case ImageProviderKey.Epic:
                    return await EpicService.GetPictures();

                case ImageProviderKey.Bing:
                    return await BingService.GetPictures();

                case ImageProviderKey.EarthView:
                    return await EarthViewService.GetPictures();

                case ImageProviderKey.OpenVerse:
                    return await OpenVerseService.GetPictures(this.randomizer);

                default:
                    break;
            }
            throw new NotImplementedException();
        }
        catch (Exception ex)
        {
            string msg = "Exception thrown: " + ex.Message +"\n" + ex;
            this.logger.Error(msg);
            throw;
        }
    }

    public async Task<byte[]> DownloadPicture(PictureMetadata picture)
    {
        if (picture.MediaType != MediaType.Image)
        {
            string msg = "Invalid media type";
            this.logger.Error(msg);
            throw new ArgumentException(msg);
        }

        string? url = picture.Url;
        if (string.IsNullOrWhiteSpace(url))
        {
            string msg = "Invalid: no media URL";
            this.logger.Error(msg);
            throw new ArgumentException(msg);            
        }

        HttpClient client = new();
        byte[] imageData = await client.GetByteArrayAsync(url);
        return imageData;
    }
}
