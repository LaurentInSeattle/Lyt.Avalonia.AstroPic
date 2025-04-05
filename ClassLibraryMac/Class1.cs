using System.Runtime.Versioning;
using AppKit;
using Foundation;

namespace ClassLibraryMac;

[SupportedOSPlatform("macOS")]
public class Class1
{
    public void DoStuff(string str)
    {
        try
        {
            var fileUrl = new NSUrl(str, isDir: false);
            var mainScreen = NSScreen.MainScreen;
            var error = new NSError();
            var key = NSWorkspace.SharedWorkspace.DesktopImageOptions; 
            var options = new NSDictionary( key, NSImageScaling.ProportionallyUpOrDown);
            NSWorkspace.SharedWorkspace.SetDesktopImageUrl(fileUrl, mainScreen, options, error); 
        }
        catch 
        { 
        }
    }
}
