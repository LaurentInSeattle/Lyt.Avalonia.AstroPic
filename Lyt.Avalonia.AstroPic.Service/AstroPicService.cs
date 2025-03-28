namespace Lyt.Avalonia.AstroPic.Service;

public class AstroPicService
{
    public static async Task<List<PictureMetadata>> GetPictures(
        Provider provider, DateTime dateTime, int count = 1)
    {
        if ((count <= 0) || (count > 8))
        {
            throw new ArgumentException("Invalid count: max == 8");
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

    public static async Task<byte[]> DownloadPicture(PictureMetadata picture)
    {
        if (picture.MediaType != MediaType.Image)
        {
            throw new ArgumentException("Invalid media type");
        }

        string? url = picture.Url;
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException("Invalid: no media URL");
        }

        HttpClient client = new();
        byte[] imageData = await client.GetByteArrayAsync(url);
        return imageData;
    }
}
