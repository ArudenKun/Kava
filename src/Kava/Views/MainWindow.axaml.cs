using Avalonia.Interactivity;
using SukiUI.Controls;

namespace Kava.Views;

public partial class MainWindow : SukiWindow
{
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        PdfViewer.PdfFile = ViewModel.PdfPath;
    }
}
