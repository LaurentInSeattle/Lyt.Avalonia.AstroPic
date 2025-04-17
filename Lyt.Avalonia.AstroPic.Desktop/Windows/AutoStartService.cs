using System;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Security.AccessControl;

using Microsoft.Win32;

namespace Lyt.Avalonia.AstroPic.Desktop.Windows;

using Lyt.Avalonia.AstroPic.Interfaces;

// using IWshRuntimeLibrary;

[SupportedOSPlatform("windows")]
public sealed class AutoStartService : IAutoStartService
{

    // C:\Users\Laurent\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup

//private void CreateShortcut()
//{
//    object shDesktop = (object)"Desktop";
//    WshShell shell = new WshShell();
//    string shortcutAddress = (string)shell.SpecialFolders.Item(ref shDesktop) + @"\Notepad.lnk";
//    IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);
//    shortcut.Description = "New shortcut for a Notepad";
//    shortcut.Hotkey = "Ctrl+Shift+N";
//    shortcut.TargetPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\notepad.exe";
//    shortcut.Save();
//}

private const string RegistryKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";

    public AutoStartService() => AutoStartService.AllowAccessToRegistry(); 

    // See:    https://stackoverflow.com/questions/7927381/programmatically-assign-the-permission-to-a-registry-subkey    
    private static void AllowAccessToRegistry ()
    {
        try
        {
            RegistryKey? key = Registry.CurrentUser.OpenSubKey(RegistryKey, writable: true);
            if (key is null)
            {
                Debug.WriteLine("No key");
                return;
            } 

            var rs = new RegistrySecurity();
            rs = key.GetAccessControl();
            string currentUserStr = Environment.UserDomainName + "\\" + Environment.UserName;
            rs.AddAccessRule(
                new RegistryAccessRule(
                    currentUserStr,
                    RegistryRights.WriteKey | RegistryRights.ReadKey | RegistryRights.Delete | RegistryRights.FullControl,
                    AccessControlType.Allow));
        }
        catch (Exception ex)
        {
            if (Debugger.IsAttached) { Debugger.Break(); }
            Debug.WriteLine(ex);
        }
    }

    public void ClearAutoStart(string applicationName)
    {
        try
        {
            RegistryKey? rk = Registry.CurrentUser.OpenSubKey(RegistryKey, writable: true);
            rk?.DeleteValue(applicationName, throwOnMissingValue: false);
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
            RegistryKey? rk = Registry.CurrentUser.OpenSubKey(RegistryKey, writable: true);
            rk?.SetValue(applicationName, applicationPath, RegistryValueKind.String);
        }
        catch (Exception ex)
        {
            if (Debugger.IsAttached) { Debugger.Break(); }
            Debug.WriteLine(ex);
        }
    }
}


/*
 * 
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace TestShortcut
{
    class Program
    {
        static void Main(string[] args)
        {
            IShellLink link = (IShellLink)new ShellLink();

            // setup shortcut information
            link.SetDescription("My Description");
            link.SetPath(@"c:\MyPath\MyProgram.exe");

            // save it
            IPersistFile file = (IPersistFile)link;
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            file.Save(Path.Combine(desktopPath, "MyLink.lnk"), false);
        }
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
} */