namespace Lyt.Avalonia.AstroPic.Model.DataObjects; 

public sealed class Picture : PictureMetadata
{
    public Rating Rating { get; set; }

    public bool Keep { get; set; }

    public bool Saved { get; set; } 

    public string? FilePath { get; set; } 

    public string Label => 
        string.Format ( "{0} ({1})" , this.Provider.ToString() , this.Date.ToShortDateString() );
}
