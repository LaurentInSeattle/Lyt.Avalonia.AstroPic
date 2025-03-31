namespace Lyt.Avalonia.AstroPic.Service; 

public class PictureMetadata
{
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

        if ( !string.IsNullOrWhiteSpace(copyright))
        {
            string[] tokens =
                copyright.Split(['(', ')'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (tokens.Length == 2)
            {
                this.Title = tokens[0];
                this.Copyright= tokens[1];
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

    public ProviderKey Provider { get; set; }

    public DateTime Date { get; set; }

    public MediaType MediaType { get; set; }

    public string? Url { get; set; } = string.Empty;

    public string? Title { get; set; } = string.Empty;

    public string? Description { get; set; } = string.Empty;

    public string? Copyright { get; set; } = string.Empty;
}
