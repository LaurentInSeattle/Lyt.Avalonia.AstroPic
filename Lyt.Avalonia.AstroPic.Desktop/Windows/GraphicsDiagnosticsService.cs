namespace Lyt.Avalonia.AstroPic.Desktop.Windows;

public static class GraphicsDiagnosticsService
{
    //public static bool TryGetActiveWin32CompositionMode([NotNullWhen(true)] out Win32CompositionMode? win32CompositionMode)
    //{
    //    if (!TryGetActiveWin32RenderingMode(out var renderingMode)
    //        || renderingMode is not Win32ActiveRenderingMode.AngleEglD3D11)
    //    {
    //        win32CompositionMode = Win32CompositionMode.RedirectionSurface;
    //        return true;
    //    }
    //    var renderTimer = AvaloniaLocator.Current.GetService<IRenderTimer>();
    //    var renderTimerClassName = renderTimer?.GetType().Name;
    //    win32CompositionMode = renderTimerClassName switch
    //    {
    //        "WinUiCompositorConnection" => Win32CompositionMode.WinUIComposition,
    //        "DirectCompositionConnection" => Win32CompositionMode.DirectComposition,
    //        "DxgiConnection" => Win32CompositionMode.LowLatencyDxgiSwapChain,
    //        _ => Win32CompositionMode.RedirectionSurface
    //    };
    //    return win32CompositionMode != null;
    //}

    //public static bool TryGetActiveWin32RenderingMode([NotNullWhen(true)] out Win32ActiveRenderingMode? win32RenderingMode)
    //{
    //    var platformGraphics = AvaloniaLocator.Current.GetService<IPlatformGraphics>();
    //    var platformGraphicsClassName = platformGraphics?.GetType().Name;
    //    win32RenderingMode = platformGraphicsClassName switch
    //    {
    //        null when AvaloniaLocator.Current.GetService<Win32PlatformOptions>()?.CustomPlatformGraphics is not null => Win32ActiveRenderingMode.Custom,
    //        null => Win32ActiveRenderingMode.Software,
    //        "D3D9AngleWin32PlatformGraphics" => Win32ActiveRenderingMode.AngleEglD3D9,
    //        "D3D11AngleWin32PlatformGraphics" => Win32ActiveRenderingMode.AngleEglD3D11,
    //        "WglPlatformOpenGlInterface" => Win32ActiveRenderingMode.Wgl,
    //        nameof(VulkanPlatformGraphics) => Win32ActiveRenderingMode.Vulkan,
    //        _ => null
    //    };

    //    return win32RenderingMode != null;
    //}

    public enum Win32ActiveRenderingMode
    {
        Custom,
        Software,
        AngleEglD3D9,
        AngleEglD3D11,
        Wgl,
        Vulkan
    }
}
