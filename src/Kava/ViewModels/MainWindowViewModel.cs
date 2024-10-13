using Kava.ViewModels.Abstractions;
using SukiUI.Dialogs;
using SukiUI.Toasts;

namespace Kava.ViewModels;

public partial class MainWindowViewModel : BaseViewModel
{
    public ISukiDialogManager DialogManager { get; } = new SukiDialogManager();
    public ISukiToastManager ToastManager { get; } = new SukiToastManager();
}
