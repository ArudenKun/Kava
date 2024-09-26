using System;
using Desktop.ViewModels.Abstractions;
using Generator.ObservableEvents;
using Material.Icons;

namespace Desktop.ViewModels.Pages;

public sealed class DashboardPageViewModel : BasePageViewModel
{
    public override int PageIndex => 1;
    public override MaterialIconKind PageIconKind => MaterialIconKind.ViewDashboard;

    public DashboardPageViewModel() { }
}
