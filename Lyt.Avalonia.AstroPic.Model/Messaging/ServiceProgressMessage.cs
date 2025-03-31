namespace Lyt.Avalonia.AstroPic.Model.Messaging;

public sealed record class ServiceProgressMessage(ProviderKey Provider, bool IsMetadata, bool IsBegin);
