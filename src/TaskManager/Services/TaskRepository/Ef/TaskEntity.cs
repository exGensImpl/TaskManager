using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManager.Models;

namespace TaskManager.Services.TaskRepository.Ef;

[Table("tasks")]
public class TaskEntity
{
  [Column("id")]
  public Guid Id { get; set; }
  
  [Column("title")]
  [StringLength(100)]
  public required string Title { get; set; }
  
  [Column("completed")]
  public bool IsCompleted { get; set; }
  
  [Column("created")]
  public DateTime CreatedAt { get; set; }
  
  [Column("deleted")]
  public bool IsDeleted { get; set; }

  public TaskItem CreateTaskItem()
  {
    return new TaskItem
    {
      Id = Id,
      Title = new(Title),
      CreatedAt = CreatedAt,
      IsCompleted = IsCompleted
    };
  }

  public static TaskEntity Create(TaskItem task)
  {
    return new TaskEntity
    {
      Id = task.Id,
      Title = task.Title,
      CreatedAt = task.CreatedAt.ToUniversalTime(),
      IsCompleted = task.IsCompleted
    };
  }
}