using System;
using System.Runtime.Versioning;
using AppKit;
using Foundation;

using Lyt.Avalonia.AstroPic.Interfaces;

#pragma warning disable IDE0130
namespace Lyt.Avalonia.AstroPic.MacOs;
#pragma warning restore IDE0130

[SupportedOSPlatform("macOS")]
public class WallpaperService() : IWallpaperService
{
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
