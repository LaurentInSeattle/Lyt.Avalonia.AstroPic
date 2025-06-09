namespace Lyt.Avalonia.AstroPic.Service;

public class AstroPicService(ILogger logger, IRandomizer randomizer)
{
    private readonly ILogger logger = logger;
    private readonly IRandomizer randomizer = randomizer;
    private readonly HttpClient httpClient = new();

    public async Task<List<PictureMetadata>> GetPictures(ImageProviderKey provider)
    {
        try
        {
            return provider switch
            {
                ImageProviderKey.Epic => await EpicService.GetPictures(),
                ImageProviderKey.Bing => await BingService.GetPictures(),
                ImageProviderKey.EarthView => await EarthViewService.GetPictures(),
                ImageProviderKey.OpenVerse => await OpenVerseService.GetPictures(this.randomizer),
                _ => await NasaService.GetPictures(), // Default, NASA 
            };

            throw new NotImplementedException(nameof(provider));
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

        return await this.httpClient.GetByteArrayAsync(url);
    }
}
