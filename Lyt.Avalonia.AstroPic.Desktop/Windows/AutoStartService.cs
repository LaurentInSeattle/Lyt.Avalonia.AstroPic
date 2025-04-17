using System;
using System.Diagnostics;
using System.Runtime.Versioning;

using Microsoft.Win32;

namespace Lyt.Avalonia.AstroPic.Desktop.Windows;

using Lyt.Avalonia.AstroPic.Interfaces;

[SupportedOSPlatform("windows")]
public sealed class AutoStartService : IAutoStartService
{
    private const string RegistryKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";

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
