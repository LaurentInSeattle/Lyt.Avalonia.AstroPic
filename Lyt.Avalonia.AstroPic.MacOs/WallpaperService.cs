using System.Runtime.Versioning;
using AppKit;
using Foundation;

using Lyt.Avalonia.AstroPic.Interfaces;

namespace Lyt.Avalonia.AstroPic.MacOs;

[SupportedOSPlatform("macOS")]
public class WallpaperService() : IWallpaperService
{
    // No mac support in .Net 9 ? 
    // Logger is .Net 9 
    // private readonly ILogger logger = logger;

    public void Set(string filePath, WallpaperStyle wallpaperStyle)
    {
        try
        {
            var fileUrl = new NSUrl(filePath, isDir: false);
            var mainScreen = NSScreen.MainScreen;
            var error = new NSError();
            var key = NSWorkspace.SharedWorkspace.DesktopImageOptions;
            var options = new NSDictionary(key, NSImageScaling.ProportionallyUpOrDown);
            NSWorkspace.SharedWorkspace.SetDesktopImageUrl(fileUrl, mainScreen, options, error);
        }
        catch(Exception ex) 
        {
            string msg = "Exception thrown on Mac OS: " + ex.ToString() ;
            // this.logger.Warning(msg);
            throw new Exception(msg);
        }

    }
}
