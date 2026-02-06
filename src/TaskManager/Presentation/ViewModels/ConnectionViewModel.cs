using System;
using System.Reactive;
using JetBrains.Annotations;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using TaskManager.Properties;
using TaskManager.Services.TaskRepository;

namespace TaskManager.Presentation.ViewModels;

public class ConnectionViewModel : ViewModelBase
{
  [Reactive] 
  [LocalizationRequired] 
  public string State { get; set; } = Resources.ConnectionViewModel_Connecting;
  
  public ReactiveCommand<Unit, Unit> Connect { get; }
  
  public ConnectionViewModel(ITaskRepository taskRepository, IDesktopServices desktopServices)
  {
    Connect = ReactiveCommand.CreateFromTask(async () =>
    {
      if (taskRepository is IInitableRepository initableRepository)
      {
        var dbJustCreated = await initableRepository.Init();

        if (dbJustCreated)
        {
          desktopServices.ShowNotification(Resources.ConnectionViewModel_DbCreated);
        }
      }
    });

    Connect.ThrownExceptions.Subscribe(_ => State = Resources.ConnectionViewModel_ConnectionError);

    desktopServices.NotifyAboutErrorsOf(Connect);
  }
}