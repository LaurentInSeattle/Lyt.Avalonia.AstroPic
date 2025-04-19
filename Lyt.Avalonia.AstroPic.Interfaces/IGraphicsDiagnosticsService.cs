using System.Diagnostics.CodeAnalysis;

namespace Lyt.Avalonia.AstroPic.Interfaces;

public enum ActiveRenderingMode
{
    Custom,
    Software,
    AngleEglD3D9,
    AngleEglD3D11,
    Wgl,
    Vulkan
}

public enum CompositionMode
{
    /// <summary>
    /// Render Avalonia to a texture inside the Windows.UI.Composition tree.
    /// </summary>
    /// <remarks>
    /// Supported on Windows 10 build 17134 and above. Ignored on other versions.
    /// This is recommended option, as it allows window acrylic effects and high refresh rate rendering.<br/>
    /// Can only be applied with <see cref="Win32PlatformOptions.RenderingMode"/>=<see cref="Win32RenderingMode.AngleEgl"/>.
    /// </remarks>
    WinUIComposition = 1,

    /// <summary>
    /// Render Avalonia to a texture inside the DirectComposition tree.
    /// </summary>
    /// <remarks>
    /// Supported on Windows 8 and above. Ignored on other versions.<br/>
    /// Can only be applied with <see cref="Win32PlatformOptions.RenderingMode"/>=<see cref="Win32RenderingMode.AngleEgl"/>.
    /// </remarks>
    DirectComposition = 2,

    /// <summary>
    /// When <see cref="LowLatencyDxgiSwapChain"/> is active, renders Avalonia through a low-latency Dxgi Swapchain.
    /// </summary>
    /// <remarks>
    /// Requires Feature Level 11_3 to be active, Windows 8.1+ Any Subversion. 
    /// This is only recommended if low input latency is desirable, and there is no need for the transparency
    /// and styling / blurring offered by <see cref="WinUIComposition"/>.<br/>
    /// Can only be applied with <see cref="Win32PlatformOptions.RenderingMode"/>=<see cref="Win32RenderingMode.AngleEgl"/>.
    /// </remarks>
    LowLatencyDxgiSwapChain = 3,

    /// <summary>
    /// The window renders to a redirection surface.
    /// </summary>
    /// <remarks>
    /// This option is kept only for compatibility with older systems. Some Avalonia features might not work.
    /// </remarks>
    RedirectionSurface,
}

public interface IGraphicsDiagnosticsService
{
    bool TryGetActiveCompositionMode([NotNullWhen(true)] out CompositionMode? compositionMode);

    bool TryGetActiveRenderingMode([NotNullWhen(true)] out ActiveRenderingMode? renderingMode);
}
