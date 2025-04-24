namespace Lyt.Avalonia.AstroPic.Messaging;

public sealed record class ZoomRequestMessage(double ZoomFactor, object? Tag = null ); 
