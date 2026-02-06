using System.Threading.Tasks;

namespace TaskManager.Services.TaskRepository;

/// <summary>
/// Репозиторий с возможностью начальной инициализации хранилища
/// </summary>
public interface IInitableRepository
{
  /// <summary>
  /// Инициализирует хранилище
  /// </summary>
  /// <returns>
  /// <see langword="true"/>, если хранилище было создано
  /// при вызове этого метода, иначе <see langword="false"/>,
  /// </returns>
  Task<bool> Init();
}