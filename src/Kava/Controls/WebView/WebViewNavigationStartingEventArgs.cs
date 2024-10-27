namespace Kava.Controls.WebView;

public class WebViewNavigationStartingEventArgs : WebViewNavigationEventArgs
{
    public bool Cancel { get; set; }

    public bool? IsRedirected { get; init; }
}
