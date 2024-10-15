using CommunityToolkit.Mvvm.ComponentModel;
using Material.Icons;

namespace Kava.ViewModels.Abstractions;

public abstract partial class BasePageViewModel : BaseViewModel, IPageViewModel
{
    [ObservableProperty]
    private bool _isPageActive;
    public virtual int PageIndex => 1;
    public virtual string PageName => GetType().Name.Replace("PageViewModel", string.Empty);
    public virtual MaterialIconKind PageIconKind => MaterialIconKind.Home;

    protected BasePageViewModel()
    {
        AttachedToVisualTree += (_, _) => IsPageActive = true;
        DetachedFromVisualTree += (_, _) => IsPageActive = false;
    }
}
