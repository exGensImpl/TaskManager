using System;
using Avalonia.Controls.Notifications;
using ReactiveUI;
using TaskManager.Properties;

namespace TaskManager.Presentation.ViewModels;

internal static class DesktopServicesExtensions
{
  public static IDisposable NotifyAboutErrorsOf(this IDesktopServices services, IReactiveCommand command)
  {
    return command.ThrownExceptions.Subscribe(
      _ => services.ShowNotification(_.Message, NotificationType.Error, Resources.Common_Error));
  }
}