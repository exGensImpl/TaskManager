using FluentAssertions;
using TaskManager.Models;
using TaskManager.Services.TaskRepository;

namespace TaskManager.Tests;

public abstract class TaskRepositoryTests
{
  protected abstract ITaskRepository CreateRepository();

  [Test]
  public async Task AddTask_ShouldContainNewTaskAfterCreation()
  {
    var repo = CreateRepository();

    var task = await repo.CreateTask("Test");

    var tasks = await repo.GetTasks();
    tasks.Any(_ => _.Id == task.Id).Should().BeTrue();
  }

  [Test]
  public async Task AddTask_WithEmptyName_ShouldThrowAnException()
  {
    var repo = CreateRepository();
    var newTaskName = string.Empty;

    await ((Func<Task<TaskItem>>)(() => repo.CreateTask(newTaskName)))
      .Should().ThrowAsync();
  }

  [Test]
  public async Task AddTask_WithTooLongName_ShouldThrowAnException()
  {
    var repo = CreateRepository();
    var newTaskName = new string(new char[200]);
    
    await ((Func<Task<TaskItem>>)(() => repo.CreateTask(newTaskName)))
      .Should().ThrowAsync();
  }

  [Test]
  public async Task AddTask_WithNullName_ShouldThrowAnException()
  {
    var repo = CreateRepository();

    await ((Func<Task<TaskItem>>)(() => repo.CreateTask(null!)))
      .Should().ThrowAsync();
  }

  [Test]
  public async Task DeleteTask_Existing_ShouldDoNotReturnTaskAfterDeletion()
  {
    var repo = CreateRepository();
    var task = await repo.CreateTask("Test");

    await repo.DeleteTask(task.Id);

    var tasks = await repo.GetTasks();
    tasks.Any(_ => _.Id == task.Id).Should().BeFalse();
  }

  [Test]
  public async Task DeleteTask_Unexisting_ShouldDoNothing()
  {
    var repo = CreateRepository();
    var unexistingId = Guid.NewGuid();

    await repo.DeleteTask(unexistingId);

    var tasks = await repo.GetTasks();
    tasks.Any(_ => _.Id == unexistingId).Should().BeFalse();
  }
}