using System;
using System.Reactive.Linq;
using ReactiveUI;

namespace TaskManager.Utils;

/// <summary>
/// Содержит методы расширения, связанные с интерфейсом <see cref="IObservable{T}"/>
/// </summary>
internal static class ObservableExtensions
{
  /// <summary>
  /// Выполняет указанную команду, перехватывая исключения, возникшие при выполнении.
  /// При применении этого метода для обработки исключений следует использовать свойство
  /// <see cref="IReactiveCommand.ThrownExceptions"/>
  /// </summary>
  /// <param name="command">Команда для выполнения</param>
  /// <typeparam name="TParam"></typeparam>
  /// <typeparam name="TResult"></typeparam>
  public static void ExecuteAndCatchErrors<TParam, TResult>(this ReactiveCommand<TParam, TResult> command)
  {
    command.Execute().Catch(Observable.Empty<TResult>()).Subscribe();
  }
}