using System.IO;
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
        PdfViewer.PdfFile = @"C:\Users\alden\Downloads\Dada-Flight-Dec17-2024-MNL-CGO.pdf";
    }
}
