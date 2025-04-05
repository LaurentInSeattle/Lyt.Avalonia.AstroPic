using Lyt.Avalonia.AstroPic.Interfaces;
using Lyt.Avalonia.Interfaces.Logger;

// using Foundation; 
// using Xamarin; 

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

/*

procedure SetDesktopBackground(const ImagePath: string; const FillColor: TAlphaColor);
var
  NSFileURL: NSURL;
  NSWorkspace1: NSWorkspace;
  MainScreen: NSScreen;
  LPointer: Pointer;
  Error: NSError;
begin
  try
    // Convert Delphi file path to NSURL
    NSFileURL := TNSURL.Wrap(TNSURL.OCClass.fileURLWithPath(StrToNSStr(ImagePath)));
    NSWorkspace1 := TNSWorkspace.Wrap(TNSWorkspace.OCClass.sharedWorkspace);
    MainScreen := TNSScreen.Wrap(TNSScreen.OCClass.mainScreen);

    LPointer := nil;
    // Set the desktop image for the main screen
    if not NSWorkspace1.setDesktopImageURL(NSFileURL, MainScreen, nil, @LPointer) then
    begin
      Error := TNSError.Wrap(LPointer);
      ShowMessage('Failed to set desktop image: ' + NSStrToStr(Error.localizedDescription));
    end;
  except
    on E: Exception do
    begin
      ShowMessage('Error setting desktop image: ' + E.Message);
    end;
  end;
end;

#!/usr/bin/env swift

import Cocoa

do {
    // get the main (currently active) screen
    if let screen = NSScreen.main {
        // get path to wallpaper file from first command line argument
        let url = URL(fileURLWithPath: CommandLine.arguments[1])
        // set the desktop wallpaper
        try NSWorkspace.shared.setDesktopImageURL(url, for: screen, options: [:])
    }
} catch {
    print(error)
} 

 struct ContentView: View {
    
    @State private var showFileImporter = false
    
    var body: some View {
        Button("Pick Background Image") {
            showFileImporter = true
        }
        .fileImporter(isPresented: $showFileImporter, allowedContentTypes: [.jpeg, .tiff, .png]) { result in
            switch result {
            case .failure(let error):
                print("Error selecting file \(error.localizedDescription)")
            case .success(let url):
                print("selected url = \(url)")
                setDesktopImage(url: url)
            }
            
        }
    }
    
    func setDesktopImage(url: URL) {
        do {
            if let screen = NSScreen.main {
                try NSWorkspace.shared.setDesktopImageURL(url, for: screen, options: [:])
            }
        } catch {
            print(error)
        }
    }
}
 
 try workspace.setDesktopImageURL(imgurl, for: screen, options: [NSWorkspace.DesktopImageOptionKey.imageScaling : 5])
//The value of the dictionary is the desired number you want to scale the image by.

 https://developer.apple.com/documentation/appkit/nsworkspace/desktopimageoptionkey/imagescaling 


 */