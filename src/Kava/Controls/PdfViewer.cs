using Avalonia.Controls;
using DependencyPropertyGenerator;
using Xilium.CefGlue.Avalonia;

namespace Kava.Controls;

[DependencyProperty("PdfFile", typeof(string))]
public partial class PdfViewer : ContentControl
{
    private const string BlankPage = "about:blank";
    private readonly AvaloniaCefBrowser _browser = new();

    public PdfViewer()
    {
        Content = _browser;
    }

    partial void OnPdfFileChanged(string? newValue)
    {
        if (newValue is null || string.IsNullOrEmpty(newValue))
        {
            _browser.Address = BlankPage;
            SetValue(PdfFileProperty, null);
            return;
        }

        _browser.Address = newValue;
    }
}
