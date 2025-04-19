using Avalonia;
using System.Runtime.CompilerServices;
using System;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.Vulkan;
using System.Diagnostics.CodeAnalysis;

using Lyt.Avalonia.AstroPic.Interfaces;
using System.Runtime.Versioning;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Lyt.Avalonia.AstroPic.Desktop.MacOs;
#pragma warning restore IDE0130 

[SupportedOSPlatform("macOS")]
public class GraphicsDiagnosticsService : IGraphicsDiagnosticsService
{
    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "get_Current")]
    static extern IAvaloniaDependencyResolver? GetCurrentAvaloniaLocator(AvaloniaLocator? nullLocator);

    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "GetService")]
    static extern object? GetAvaloniaDependencyService(IAvaloniaDependencyResolver? avaloniaLocator, Type serviceType);

    private static IAvaloniaDependencyResolver? AvaloniaLocator { get; } = GetCurrentAvaloniaLocator(null);

    private static T? GetAvaloniaLocatorService<T>()
        where T : class
    {
        if (AvaloniaLocator is null)
        {
            return null;
        }

        object? result = GetAvaloniaDependencyService(AvaloniaLocator, typeof(T));
        return result as T;
    }

    public bool TryGetActiveCompositionMode([NotNullWhen(true)] out CompositionMode? win32CompositionMode)
    {
        if (!this.TryGetActiveRenderingMode(out ActiveRenderingMode? renderingMode)
            || renderingMode is not ActiveRenderingMode.AngleEglD3D11)
        {
            win32CompositionMode = CompositionMode.RedirectionSurface;
            return true;
        }

        IRenderTimer? renderTimer = GetAvaloniaLocatorService<IRenderTimer>();
        string? renderTimerClassName = renderTimer?.GetType().Name;
        win32CompositionMode = renderTimerClassName switch
        {
            "WinUiCompositorConnection" => CompositionMode.WinUIComposition,
            "DirectCompositionConnection" => CompositionMode.DirectComposition,
            "DxgiConnection" => CompositionMode.LowLatencyDxgiSwapChain,
            _ => CompositionMode.RedirectionSurface
        };

        return win32CompositionMode != null;
    }

    public bool TryGetActiveRenderingMode([NotNullWhen(true)] out ActiveRenderingMode? win32RenderingMode)
    {
        IPlatformGraphics? platformGraphics = GetAvaloniaLocatorService<IPlatformGraphics>();
        string? platformGraphicsClassName = platformGraphics?.GetType().Name;
        win32RenderingMode = platformGraphicsClassName switch
        {
            null when GetAvaloniaLocatorService<Win32PlatformOptions>()?.CustomPlatformGraphics is not null => ActiveRenderingMode.Custom,
            null => ActiveRenderingMode.Software,
            "D3D9AngleWin32PlatformGraphics" => ActiveRenderingMode.AngleEglD3D9,
            "D3D11AngleWin32PlatformGraphics" => ActiveRenderingMode.AngleEglD3D11,
            "WglPlatformOpenGlInterface" => ActiveRenderingMode.Wgl,
            nameof(VulkanPlatformGraphics) => ActiveRenderingMode.Vulkan,
            _ => null
        };

        return win32RenderingMode != null;
    }
}
