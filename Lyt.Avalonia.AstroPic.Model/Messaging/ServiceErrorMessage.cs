namespace Lyt.Avalonia.AstroPic.Model.Messaging;

public sealed record class ServiceErrorMessage(ImageProviderKey Provider, string ErrorKey= ""); 
