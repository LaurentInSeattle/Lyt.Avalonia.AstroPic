namespace Lyt.Avalonia.AstroPic.Service;

public class AstroPicService
{
    private readonly IMessenger messenger;
    private readonly ILogger logger;

    public AstroPicService(IMessenger messenger, ILogger logger)
    {
        this.messenger = messenger;
        this.logger = logger;
    }

    public async Task<List<PictureMetadata>> GetPictures(
        Provider provider, DateTime dateTime, int count = 1)
    {
        if ((count <= 0) || (count > 8))
        {
            string msg = "Invalid count: max == 8";
            this.logger.Error(msg);
            throw new ArgumentException(msg);
        }

        switch (provider)
        {
            case Provider.Nasa:
                return await NasaService.GetPictures(dateTime);

            case Provider.Bing:
                return await BingService.GetPictures(dateTime);

            case Provider.EarthView:
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
