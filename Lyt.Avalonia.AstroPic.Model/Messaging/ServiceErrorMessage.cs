namespace Lyt.Avalonia.AstroPic.Model.Messaging;

public sealed record class ServiceErrorMessage(ProviderKey Provider, string ErrorKey= ""); 
