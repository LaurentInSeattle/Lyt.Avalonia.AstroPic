namespace Lyt.Avalonia.AstroPic.Interfaces;

public interface IAutoStartService
{
    void SetAutoStart(string applicationName, string applicationPath);

    void ClearAutoStart(string applicationName);
}
