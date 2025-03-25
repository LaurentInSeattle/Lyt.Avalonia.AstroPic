namespace Lyt.Avalonia.AstroPic.Service; 

public sealed class Picture
{
    internal Picture(BingPicture bingPicture)
    {
        this.Provider = Provider.Bing;
        this.Date = DateTime.Now.Date;
        this.MediaType = MediaType.Image;

        this.Url = string.Concat(BingService.Endpoint, bingPicture.PartialUrl);

        this.Title = bingPicture.Title;
        this.Description = bingPicture.Copyright;
        this.Copyright = bingPicture.Copyright;
    }

    internal Picture(NasaPicture nasaPicture)
    {
        this.Provider = Provider.Nasa;
        this.Date = DateTime.Parse(nasaPicture.Date);
        this.MediaType = nasaPicture.MediaType == "image" ? MediaType.Image : MediaType.Video;
        this.Url = nasaPicture.HdImageUrl;
        this.Title = nasaPicture.Title;
        this.Description = nasaPicture.Explanation;
        this.Copyright = nasaPicture.Copyright; 
    }

    public Provider Provider { get; set; }

    public DateTime Date { get; set; }

    public MediaType MediaType { get; set; }

    public string? Url { get; set; } = string.Empty;

    public string? Title { get; set; } = string.Empty;

    public string? Description { get; set; } = string.Empty;

    public string? Copyright { get; set; } = string.Empty;
}
