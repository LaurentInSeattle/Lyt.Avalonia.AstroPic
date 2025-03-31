namespace Lyt.Avalonia.AstroPic.Model.DataObjects; 

public sealed class Picture
{
    public Picture() { /* Required for serialization */ }

    public Picture(PictureMetadata pictureMetadata)
    {
        this.PictureMetadata = pictureMetadata;
        this.SetImageFilePaths(); 
    }

    [JsonRequired]
    public PictureMetadata PictureMetadata { get; set; } = new(); 

    [JsonRequired]
    public Rating Rating { get; set; } = Rating.Normal;

    [JsonRequired]
    public bool Keep { get; set; } = false;

    [JsonRequired]
    public string ImageFilePath { get; set; } = string.Empty;

    [JsonRequired]
    public string ThumbnailFilePath { get; set; } = string.Empty ;

    public string Label => 
        string.Format ( 
            "{0} ({1})" , 
            this.PictureMetadata.Provider.ToString() , 
            this.PictureMetadata.Date.ToShortDateString() );

    private void SetImageFilePaths ()
    {
        var meta = this.PictureMetadata;
        var date = meta.Date; 
        this.ImageFilePath = 
            string.Format(
                "{0}_{1}_{2}_{3}.jpg", meta.Provider.ToString(), date.Year, date.Month, date.Day);
        this.ThumbnailFilePath =
            string.Format(
                "{0}_{1}_{2}_{3}_Thumb.jpg", meta.Provider.ToString(), date.Year, date.Month, date.Day);
    }
}
