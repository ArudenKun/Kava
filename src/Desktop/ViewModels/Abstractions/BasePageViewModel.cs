using CommunityToolkit.Mvvm.ComponentModel;
using Core.Extensions;
using Generator.ObservableEvents;
using Material.Icons;
using R3;

namespace Desktop.ViewModels.Abstractions;

public abstract partial class BasePageViewModel : BaseViewModel, IPageViewModel
{
    [ObservableProperty]
    private bool _isPageActive;
    public virtual int PageIndex => 1;
    public virtual string PageName => GetType().Name.Replace("PageViewModel", string.Empty);
    public virtual MaterialIconKind PageIconKind => MaterialIconKind.Home;

    protected BasePageViewModel()
    {
        this.Events()
            .AttachedToVisualTree.Subscribe(_ => IsPageActive = true)
            .DisposeWith(Subscriptions);

        this.Events()
            .DetachedFromVisualTree.Subscribe(_ => IsPageActive = false)
            .DisposeWith(Subscriptions);
    }
}
