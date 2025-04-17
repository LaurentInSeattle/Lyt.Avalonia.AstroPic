using System.Runtime.Versioning;

#pragma warning disable IDE0130
namespace Lyt.Avalonia.AstroPic.MacOs;
#pragma warning restore IDE0130

using Lyt.Avalonia.AstroPic.Interfaces;

[SupportedOSPlatform("macOS")]
public sealed class AutoStartService : IAutoStartService
{
    public void ClearAutoStart(string applicationName, string applicationPath)
    {
        // TODO 
    }

    public void SetAutoStart(string applicationName, string applicationPath)
    {
        // TODO 
    }
}
