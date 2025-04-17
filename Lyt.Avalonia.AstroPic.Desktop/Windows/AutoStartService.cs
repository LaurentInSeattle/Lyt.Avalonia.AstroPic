using System;
using System.Diagnostics;
using System.Runtime.Versioning;

using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Lyt.Avalonia.AstroPic.Desktop.Windows;

using Lyt.Avalonia.AstroPic.Interfaces;

[SupportedOSPlatform("windows")]
public sealed class AutoStartService : IAutoStartService
{
    public AutoStartService() { }

    public void ClearAutoStart(string applicationName, string applicationPath)
    {
        try
        {
            string startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            string shortcutPath = Path.Combine(startupPath, applicationName + ".lnk");
            if (!File.Exists(shortcutPath))
            {
                return;
            }

            // File.Delete(shortcutPath); 
        }
        catch (Exception ex)
        {
            if (Debugger.IsAttached) { Debugger.Break(); }
            Debug.WriteLine(ex);
        }
    }

    public void SetAutoStart(string applicationName, string applicationPath)
    {
        try
        {
            CreateShortcut(applicationName, applicationPath); 
        }
        catch (Exception ex)
        {
            if (Debugger.IsAttached) { Debugger.Break(); }
            Debug.WriteLine(ex);
        }
    }

    public static void CreateShortcut (string applicationName, string applicationPath)
    {
        string startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
        string shortcutPath = Path.Combine(startupPath, applicationName + ".lnk"); 
        if ( File.Exists(shortcutPath) )
        {
            return;
        }

        // setup shortcut information
        if ( applicationPath.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase))
        {
            applicationPath = applicationPath.Replace(".dll", ".exe"); 
        }

        var link = (IShellLink)new ShellLink();
        link.SetDescription(applicationName);
        link.SetPath(applicationPath);
        var file = (IPersistFile)link;
        file.Save(shortcutPath, false);
    }


    [ComImport]
    [Guid("00021401-0000-0000-C000-000000000046")]
    internal class ShellLink
    {
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("000214F9-0000-0000-C000-000000000046")]
    internal interface IShellLink
    {
        void GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, out IntPtr pfd, int fFlags);
        void GetIDList(out IntPtr ppidl);
        void SetIDList(IntPtr pidl);
        void GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cchMaxName);
        void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
        void GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);
        void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
        void GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);
        void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
        void GetHotkey(out short pwHotkey);
        void SetHotkey(short wHotkey);
        void GetShowCmd(out int piShowCmd);
        void SetShowCmd(int iShowCmd);
        void GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cchIconPath, out int piIcon);
        void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
        void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);
        void Resolve(IntPtr hwnd, int fFlags);
        void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
    }
}
