﻿namespace Kava.Controls.WebView;

public class WebViewNavigationCompletedEventArgs : WebViewNavigationEventArgs
{
    public bool IsSuccess { get; init; } = true;
}
