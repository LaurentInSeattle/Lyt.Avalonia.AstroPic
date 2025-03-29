namespace Lyt.Avalonia.AstroPic.Model.Messaging;

public sealed record class ServiceErrorMessage(Provider Provider, string ErrorKey= ""); 
