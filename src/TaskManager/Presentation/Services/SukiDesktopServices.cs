using System;
using Avalonia.Controls.Notifications;
using Avalonia.Threading;
using SukiUI.Toasts;

namespace TaskManager.Presentation.ViewModels;

/// <summary>
/// Предоставляет возможности графического интерфейса пользователя, используя библиотеку <see cref="SukiUI"/>
/// </summary>
/// <param name="ToastManager"></param>
public sealed record SukiDesktopServices(ISukiToastManager ToastManager) : IDesktopServices
{
  /// <inheritdoc/>
  public void ShowNotification(
    string content, 
    NotificationType type = NotificationType.Information,
    string? title = null)
  {
    if (Dispatcher.UIThread.CheckAccess() == false)
    {
      Dispatcher.UIThread.Invoke(() => ShowNotification(content, type, title));
      return;
    }

    ToastManager
      .CreateToast()
      .OfType(type)
      .WithTitle(title ?? string.Empty)
      .WithContent(content)
      .Dismiss().ByClicking()
      .Dismiss().After(TimeSpan.FromSeconds(5))
      .Queue();
  }
}