namespace Lyt.Avalonia.AstroPic.Service;

public class AstroPicService(IMessenger messenger, ILogger logger)
{
    private readonly IMessenger messenger = messenger;
    private readonly ILogger logger = logger;

    public async Task<List<PictureMetadata>> GetPictures(
        ProviderKey provider, DateTime dateTime, int count = 1)
    {
        if ((count <= 0) || (count > 8))
        {
            string msg = "Invalid count: max == 8";
            this.logger.Error(msg);
            throw new ArgumentException(msg);
        }

        switch (provider)
        {
            case ProviderKey.Nasa:
                return await NasaService.GetPictures(dateTime);

            case ProviderKey.Bing:
                return await BingService.GetPictures(dateTime);

            case ProviderKey.EarthView:
                return await EarthViewService.GetPictures();

            default:
                break;
        }

        throw new NotImplementedException();
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
