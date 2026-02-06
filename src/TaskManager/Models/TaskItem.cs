using System;

namespace TaskManager.Models;

public class TaskItem
{
  public Guid Id { get; init; }
  
  public required string Title { get; init; }
  
  public bool IsCompleted { get; set; }
  
  public DateTime CreatedAt { get; init; }

  public static TaskItem Create(string title)
  {
    if (string.IsNullOrWhiteSpace(title) || title.Length > 100)
    {
      throw new ArgumentException("Incorrect task name");
    }
    
    return new()
    {
      Id = Guid.NewGuid(),
      Title = new(title),
      CreatedAt = DateTime.Now
    };
  }
}