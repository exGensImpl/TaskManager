using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using TaskManager.Models;
using TaskManager.Services.TaskRepository;
using TaskManager.Utils;

namespace TaskManager.Presentation.ViewModels;

public class TaskListViewModel : ViewModelBase
{
  public ReactiveCommand<Unit, Unit> LoadTasks { get; }

  public IReactiveCommand AddTask { get; }

  public ReactiveCommand<TaskItem, Unit> DeleteTask { get; }

  public ReactiveCommand<TaskItem, Unit> ToggleTaskCompleted { get; }

  public ObservableCollection<TaskItem> Tasks { get; } = [];

  [Reactive]
  public string? NewTaskName { get; set; }

  private readonly ITaskRepository _taskRepository;

  public TaskListViewModel(ITaskRepository taskRepository, IDesktopServices desktopServices)
  {
    _taskRepository = taskRepository;

    LoadTasks = ReactiveCommand.CreateFromTask<Unit, Unit>(DoLoadTasks);
    AddTask = ReactiveCommand.CreateFromTask(DoAddTask, CanExecuteAddTask());
    DeleteTask = ReactiveCommand.CreateFromTask<TaskItem, Unit>(DoDeleteTask);
    ToggleTaskCompleted = ReactiveCommand.CreateFromTask<TaskItem, Unit>(
      async taskItem =>
      {
        try
        {
          await _taskRepository.SetCompleted(taskItem.Id, taskItem.IsCompleted);
        }
        catch
        {
          taskItem.IsCompleted = !taskItem.IsCompleted;
          throw;
        }

        return Unit.Default;
      }
    );

    desktopServices.NotifyAboutErrorsOf(LoadTasks);
    desktopServices.NotifyAboutErrorsOf(AddTask);
    desktopServices.NotifyAboutErrorsOf(DeleteTask);
    desktopServices.NotifyAboutErrorsOf(ToggleTaskCompleted);
    
    this.ValidationRule(
      viewModel => viewModel.NewTaskName,
      // Случай с null считается валидным чтобы постоянно не отображалась ошибка в поле ввода названия задачи
      name => name is null || (name.IsNotNullOrSpaces() && name.Length <= 100),
      "You must specify a valid name");
  }

  private async Task<Unit> DoLoadTasks(Unit _, CancellationToken token)
  {
    Tasks.AddRange(await _taskRepository.GetTasks(token));
    
    return Unit.Default;
  }

  private async Task DoAddTask()
  {
    if (NewTaskName is null)
    {
      return;
    }
    
    var newTask = await _taskRepository.CreateTask(NewTaskName);
    
    Tasks.Add(newTask);
    NewTaskName = null;
  }

  private IObservable<bool> CanExecuteAddTask()
  {
    return this.WhenAnyValue(_ => _.NewTaskName).Select(_ => _.IsNotNullOrSpaces());
  }

  private async Task<Unit> DoDeleteTask(TaskItem task)
  {
    await _taskRepository.DeleteTask(task.Id);
    Tasks.Remove(task);
    
    return Unit.Default;
  }
}