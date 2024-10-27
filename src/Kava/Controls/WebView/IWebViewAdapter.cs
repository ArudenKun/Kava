using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Platform;

namespace Kava.Controls.WebView;

public interface IWebViewAdapter : IWebView, IDisposable, IPlatformHandle
{
    Task SetParentAsync(IntPtr handle);

    void HandleSizeChanged(Size newSize);
}
