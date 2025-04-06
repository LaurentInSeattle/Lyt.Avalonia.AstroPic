using Microsoft.Win32;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

#pragma warning disable IDE0130
namespace Lyt.Avalonia.AstroPic.Windows;
#pragma warning restore IDE0130

using Lyt.Avalonia.AstroPic.Interfaces;

[SupportedOSPlatform("windows")]
public class WallpaperService : IWallpaperService
{
    private const int SPI_SETDESKWALLPAPER = 20;
    private const int SPIF_UPDATEINIFILE = 0x01;
    private const int SPIF_SENDWININICHANGE = 0x02;

#pragma warning disable IDE0079
#pragma warning disable SYSLIB1054
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
#pragma warning restore SYSLIB1054
#pragma warning restore IDE0079

    public WallpaperService()
    {
        // An empty CTOR is required for the Activator 
    }

    [SupportedOSPlatform("windows")]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(WallpaperService))]
    public void Set(string filePath, WallpaperStyle wallpaperStyle) 
    {
        RegistryKey? maybeKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
        if (maybeKey is RegistryKey key)
        {
            if (wallpaperStyle == WallpaperStyle.Stretched)
            {
                key.SetValue(@"WallpaperStyle", 2.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }
            else if (wallpaperStyle == WallpaperStyle.Centered)
            {
                key.SetValue(@"WallpaperStyle", 1.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }
            else if (wallpaperStyle == WallpaperStyle.Tiled)
            {
                key.SetValue(@"WallpaperStyle", 1.ToString());
                key.SetValue(@"TileWallpaper", 1.ToString());
            }
            else if (wallpaperStyle == WallpaperStyle.Fit)
            {
                key.SetValue(@"WallpaperStyle", 6.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }
            else if (wallpaperStyle == WallpaperStyle.Fill)
            {
                key.SetValue(@"WallpaperStyle", 10.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }
            else
            {
                string msg = "Unsupported wallpaper style. "; 
                // this.logger.Warning(msg);
                throw new Exception(msg);
            }

            try
            {
                _ = SystemParametersInfo(
                        SPI_SETDESKWALLPAPER,
                        0,
                        filePath,
                        SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
            } 
            catch (Exception ex)
            {
                string msg = "Failed to set SPI_SETDESKWALLPAPER" + ex.Message;
                // this.logger.Warning(msg);
                throw new Exception(msg);
            }
        }
        else
        {
            string msg = "Failed to retrieve registry key 'Control Panel\\Desktop'. ";
            // this.logger.Warning(msg);
            throw new Exception(msg);             
        }
    }
}
