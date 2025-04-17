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

    /*
     * 

See:    https://stackoverflow.com/questions/7927381/programmatically-assign-the-permission-to-a-registry-subkey

Microsoft.Win32.RegistryKey key;
key = Microsoft.Win32.Registry.LocalMachine;
RegistrySecurity rs = new RegistrySecurity();
rs = key.GetAccessControl();
string currentUserStr = Environment.UserDomainName + "\\" + Environment.UserName;
rs.AddAccessRule(
    new RegistryAccessRule(
        currentUserStr, 
        RegistryRights.WriteKey 
        | RegistryRights.ReadKey 
        | RegistryRights.Delete 
        | RegistryRights.FullControl, 
        AccessControlType.Allow));

     */

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
