namespace Lyt.Avalonia.AstroPic.Model.Messaging;

public sealed record class ServiceProgressMessage(ImageProviderKey Provider, bool IsMetadata, bool IsBegin);
