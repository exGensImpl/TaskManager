using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManager.Models;

namespace TaskManager.Services.TaskRepository.Ef;

internal sealed class EntityFrameworkTaskRepository(
  IDbContextFactory<TasksContext> contextFactory,
  ILogger<EntityFrameworkTaskRepository> logger)
  : ITaskRepository, IInitableRepository
{
  public async Task<bool> Init()
  {
    await using var context = await contextFactory.CreateDbContextAsync();
    return await context.Database.EnsureCreatedAsync();
  }

  public async Task<IReadOnlyCollection<TaskItem>> GetTasks(CancellationToken token)
  {
    await using var context = await contextFactory.CreateDbContextAsync(token);
    var entities = await context.Tasks.Where(_ => _.IsDeleted == false).ToArrayAsync(token);

    return entities.Select(_ => _.CreateTaskItem()).ToArray();
  }

  public async Task SetCompleted(Guid id, bool isCompleted)
  {
    await using var context = await contextFactory.CreateDbContextAsync();
    
    await context
      .Tasks
      .Where(_ => _.Id == id)
      .ExecuteUpdateAsync(_ => _.SetProperty(_ => _.IsCompleted, isCompleted));
    
    logger.LogInformation("Task {TaskID} completed changed to: {IsCompleted}", id, isCompleted);
  }

  public async Task DeleteTask(Guid id)
  {
    await using var context = await contextFactory.CreateDbContextAsync();
    
    await context
      .Tasks
      .Where(_ => _.Id == id)
      .ExecuteUpdateAsync(_ => _.SetProperty(_ => _.IsDeleted, true));
    
    logger.LogInformation("Task {TaskID} deleted", id);
  }

  public async Task<TaskItem> CreateTask(string name)
  {
    var newTask = TaskItem.Create(name);
    
    await using var context = await contextFactory.CreateDbContextAsync();
    context.Tasks.Add(TaskEntity.Create(newTask));
    await context.SaveChangesAsync();

    logger.LogInformation("Task {TaskId} added", newTask.Id);
    
    return newTask;
  }
}