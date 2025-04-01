namespace Lyt.Avalonia.AstroPic.Service;

public class PictureMetadata
{
    private static readonly List<string> SupportedPicturesFileExtensions =
    [
        "jpg",
        "jpeg",
        "JPG",
        "JPEG",
        "png",
        "PNG"
    ];

    public PictureMetadata()
    {
        this.Provider = ProviderKey.Unknown;
        this.Date = DateTime.Now.Date;
        this.MediaType = MediaType.Image;
    }

    internal PictureMetadata(EarthViewPicture earthViewPicture)
    {
        this.Provider = ProviderKey.EarthView;
        this.Date = DateTime.Now.Date;
        this.MediaType = MediaType.Image;
        this.Url = earthViewPicture.PhotoUrl;
        this.Title = earthViewPicture.Title;
        this.Title = this.Title.Replace("\u2013 Earth View from Google", string.Empty);
        this.Description = string.Empty;
        this.Copyright = earthViewPicture.Copyright;
    }

    internal PictureMetadata(BingPicture bingPicture)
    {
        this.Provider = ProviderKey.Bing;
        this.Date = DateTime.Now.Date;
        this.MediaType = MediaType.Image;
        this.Url = string.Concat(BingService.Endpoint, bingPicture.PartialUrl);

        string? copyright = bingPicture.Copyright;
        this.Title = copyright;
        this.Copyright = copyright;
        this.Description = bingPicture.Title;

        if (!string.IsNullOrWhiteSpace(copyright))
        {
            string[] tokens =
                copyright.Split(['(', ')'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (tokens.Length == 2)
            {
                this.Title = tokens[0];
                this.Copyright = tokens[1];
            }
        }
    }

    internal PictureMetadata(NasaPicture nasaPicture)
    {
        this.Provider = ProviderKey.Nasa;
        this.Date = DateTime.Parse(nasaPicture.Date);
        this.MediaType = nasaPicture.MediaType == "image" ? MediaType.Image : MediaType.Video;
        this.Url = nasaPicture.HdImageUrl;
        this.Title = nasaPicture.Title;
        this.Description = nasaPicture.Explanation;
        this.Copyright = nasaPicture.Copyright;
    }

    public string? UrlFileExtension()
    {
        if ((this.MediaType != MediaType.Image) || string.IsNullOrWhiteSpace(this.Url))
        {
            return string.Empty;
        }

        string[] tokens = this.Url.Split(['.'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        string maybeExtension = tokens[^1];
        if (SupportedPicturesFileExtensions.Contains(maybeExtension) ) 
        {
            return maybeExtension;
        }

        foreach (string extension in SupportedPicturesFileExtensions)
        {
            if (maybeExtension.Contains(extension, StringComparison.InvariantCultureIgnoreCase))
            {
                return extension;
            }
        }

        return "jpg"; 
    }

    public string TodayImageFilePath()
    {
        string? maybeExtension = this.UrlFileExtension();
        string extension = string.IsNullOrWhiteSpace(maybeExtension) ? "jpg" : maybeExtension;
        return string.Format("{0}_Today.{1}", this.Provider.ToString(), extension);
    }

    [JsonRequired]
    public ProviderKey Provider { get; set; }

    [JsonRequired]
    public DateTime Date { get; set; }

    [JsonRequired]
    public MediaType MediaType { get; set; }

    [JsonRequired]
    public string? Url { get; set; } = string.Empty;

    [JsonRequired]
    public string? Title { get; set; } = string.Empty;

    [JsonRequired]
    public string? Description { get; set; } = string.Empty;

    [JsonRequired]
    public string? Copyright { get; set; } = string.Empty;
}
