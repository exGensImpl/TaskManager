using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace TaskManager.Services.TaskRepository.Ef;

public sealed class TasksContext(
  DbContextOptions<TasksContext> options, 
  ILoggerFactory? loggerFactory = null)
  : DbContext(options)
{
  public DbSet<TaskEntity> Tasks { get; set; }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder
      .UseLoggerFactory(loggerFactory);
}