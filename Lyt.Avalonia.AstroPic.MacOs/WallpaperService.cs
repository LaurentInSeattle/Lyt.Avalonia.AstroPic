using Lyt.Avalonia.AstroPic.Interfaces;
using Lyt.Avalonia.Interfaces.Logger;

namespace Lyt.Avalonia.AstroPic.MacOs;

public class WallpaperService(ILogger logger) : IWallpaperService
{
    private readonly ILogger logger = logger;

    public void Set(string filePath, WallpaperStyle wallpaperStyle)
    {
        string msg = "Not implemented (yet) on Mac OS. ";
        this.logger.Warning(msg);
        throw new Exception(msg);
    }
}
