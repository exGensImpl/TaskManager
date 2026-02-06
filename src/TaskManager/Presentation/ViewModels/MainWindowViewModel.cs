using ReactiveUI.Fody.Helpers;
using SukiUI.Toasts;

namespace TaskManager.Presentation.ViewModels;

internal class MainWindowViewModel : ViewModelBase
{
  public ISukiToastManager? ToastManager { get; }
  
  [Reactive]
  public object? Content { get; set; }

  public MainWindowViewModel(IDesktopServices desktopServices)
  {
    if (desktopServices is SukiDesktopServices sukiDesktopServices)
    {
      ToastManager = sukiDesktopServices.ToastManager;
    }
  }
}