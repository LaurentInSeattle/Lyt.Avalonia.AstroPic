using System.Globalization;
using static System.Net.WebRequestMethods;

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

    internal PictureMetadata(OpenVersePicture openVersePicture)
    {
        this.Provider = ProviderKey.OpenVerse;
        this.Date = DateTime.Parse(openVersePicture.Date);
        this.MediaType = MediaType.Image ;
        this.Url = openVersePicture.Url;
        this.Title = openVersePicture.Title;
        this.Copyright = string.Format("Copyright / Author: {0}" ,  openVersePicture.Creator);
        this.Description = openVersePicture.Attribution; 

        // Override date, so that it is one of today's picture 
        this.Date = DateTime.Now.Date;
    }

    internal PictureMetadata(EpicPicture epicPicture)
    {
        // Example URL 
        //  https://api.nasa.gov/EPIC/archive/natural/2019/05/30/png/epic_1b_20190530011359.png?api_key=DEMO_KEY
        const string urlFormat =
            "https://api.nasa.gov/EPIC/archive/natural/{0:D4}/{1:D2}/{2:D2}/png/{3}.png?api_key=DEMO_KEY";
        try
        {
            // Can be bad ? 
            string? photoPartialUrl = epicPicture.PhotoPartialUrl; 

            // Date as: year, month, day followed by time: "2025-04-17 00:03:42"
            // This is the date of the day when the picture was taken
            // We use it for URL and title 
            string date = epicPicture.Date[..10];
            string[] tokens = date.Split(['-'], StringSplitOptions.RemoveEmptyEntries);
            if ((tokens.Length == 3) && (!string.IsNullOrWhiteSpace(photoPartialUrl)))
            {
                int year = int.Parse(tokens[0]);
                int month = int.Parse(tokens[1]);
                int day = int.Parse(tokens[2]);
                this.Date = new DateTime(year, month, day);
                this.Url = string.Format(urlFormat, year, month, day, epicPicture.PhotoPartialUrl);
                this.Title = this.Date.ToLongDateString();

                // Override date, so that it is one of today's picture 
                this.Date = DateTime.Now.Date;
            }
            else
            {
                this.Title = epicPicture.Title;
                throw new Exception("Bad date or bad url");
            } 
        } 
        catch (Exception ex) 
        {
            Debug.WriteLine(ex);
            this.Date = DateTime.Parse("05/12/1958", new CultureInfo ("en-US"));
            this.Url = string.Empty;
        }

        this.Provider = ProviderKey.Epic;
        this.MediaType = MediaType.Image ;
        this.Description = string.Empty;
        this.Copyright = epicPicture.Copyright;
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
