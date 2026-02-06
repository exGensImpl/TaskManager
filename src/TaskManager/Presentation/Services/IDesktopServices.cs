using Avalonia.Controls.Notifications;
using JetBrains.Annotations;

namespace TaskManager.Presentation.ViewModels;

/// <summary>
/// Предоставляет сервис по взаимодействию с графическим интерфейсом десктопного приложения
/// </summary>
public interface IDesktopServices
{
  /// <summary>
  /// Выводит уведомление
  /// </summary>
  /// <param name="content">Текст уведомления</param>
  /// <param name="type">Уровень важности уведомления</param>
  /// <param name="title">Заголовок уведомления</param>
  void ShowNotification(
    [LocalizationRequired] string content, 
    NotificationType type = NotificationType.Information,
    [LocalizationRequired] string? title = null);
}