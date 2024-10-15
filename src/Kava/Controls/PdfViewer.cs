using System.IO;
using Avalonia.Controls;
using DependencyPropertyGenerator;
using Kava.Controls.WebView;

namespace Kava.Controls;

[DependencyProperty<string>("PdfFile")]
public sealed partial class PdfViewer : ContentControl
{
    private static readonly Uri BlankPage = new("about:blank");
    private readonly NativeWebView _webView = new();

    public PdfViewer()
    {
        Content = _webView;
    }

    partial void OnPdfFileChanged(string? newValue)
    {
        if (string.IsNullOrEmpty(newValue) || !File.Exists(newValue))
        {
            _webView.Navigate(BlankPage);
            return;
        }

        _webView.Navigate(new Uri(newValue));
    }
}
