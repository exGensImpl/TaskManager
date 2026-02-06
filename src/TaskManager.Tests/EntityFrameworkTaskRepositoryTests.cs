using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManager.Services.TaskRepository;
using TaskManager.Services.TaskRepository.Ef;

namespace TaskManager.Tests;

public sealed class EntityFrameworkTaskRepositoryTests : TaskRepositoryTests
{
  private readonly ILogger<EntityFrameworkTaskRepository> _stubLogger
    = new LoggerFactory().CreateLogger<EntityFrameworkTaskRepository>();
  
  private SqliteConnection _connection;
  private DbContextOptions<TasksContext> _contextOptions;
  private Mock<IDbContextFactory<TasksContext>> _contextFactoryMock;

  [OneTimeSetUp]
  public void OneTimeSetUp()
  {
    SQLitePCL.Batteries_V2.Init();

    _contextFactoryMock = new Mock<IDbContextFactory<TasksContext>>();
    
    _contextFactoryMock
      .Setup(factory => factory.CreateDbContext())
      .Returns(() => new TasksContext(_contextOptions));
    
    _contextFactoryMock
      .Setup(factory => factory.CreateDbContextAsync(It.IsAny<CancellationToken>()))
      .Returns(() => Task.FromResult(new TasksContext(_contextOptions)));
  }
  
  [SetUp]
  public async Task Setup()
  {
    _connection = new SqliteConnection("Filename=:memory:");
    _connection.Open();

    _contextOptions = new DbContextOptionsBuilder<TasksContext>()
      .UseSqlite(_connection)
      .Options;

    // БД создаётся не через EntityFrameworkTaskRepository,
    // чтобы каждый тест использовал только один метод тестируемого класса
    await new TasksContext(_contextOptions).Database.EnsureCreatedAsync();
  }
  
  protected override ITaskRepository CreateRepository()
  {
    return new EntityFrameworkTaskRepository(_contextFactoryMock.Object, _stubLogger);
  }

  [TearDown]
  public void TearDown()
  {
    // Connection необходимо закрывать после каждого теста для очистки базы данных
    _connection.Dispose();
  }
}