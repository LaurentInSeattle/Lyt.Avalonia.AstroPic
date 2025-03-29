namespace Lyt.Avalonia.AstroPic.Model.Messaging;

public sealed record class ServiceProgressMessage(Provider Provider, bool IsMetadata, bool IsBegin);
