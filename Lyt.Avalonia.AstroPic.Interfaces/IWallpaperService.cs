namespace Lyt.Avalonia.AstroPic.Interfaces;

// Needs to be a .Net 8 project as it seems like there is no .Net on Mac just yet 
public interface IWallpaperService
{
    void Set(string filePath, WallpaperStyle wallpaperStyle);
}
