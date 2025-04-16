namespace Lyt.Avalonia.AstroPic.Messaging;

public sealed record class ViewActivationMessage(
    ViewActivationMessage.ActivatedView View, object? ActivationParameter = null)
{
    public enum ActivatedView
    {
        Intro,
        Gallery,
        Collection,
        Settings,

        GoBack,
        Exit,
    }
}
