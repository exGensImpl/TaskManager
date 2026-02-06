using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using TaskManager.Models;

namespace TaskManager.Services.TaskRepository;

/// <summary>
/// Предоставляет доступ к сохранённому списку задач,
/// позволяя их добавление удаление и отметку как выполненных
/// </summary>
public interface ITaskRepository
{
  [MustUseReturnValue]
  Task<IReadOnlyCollection<TaskItem>> GetTasks(CancellationToken token = default);

  Task<TaskItem> CreateTask(string name);

  Task SetCompleted(Guid id, bool isCompleted);

  Task DeleteTask(Guid id);
}