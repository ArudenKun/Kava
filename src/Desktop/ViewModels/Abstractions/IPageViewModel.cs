using Material.Icons;

namespace Desktop.ViewModels.Abstractions;

public interface IPageViewModel : IViewModel
{
    bool IsPageActive { get; set; }

    int PageIndex { get; }

    string PageName { get; }

    MaterialIconKind PageIconKind { get; }
}
