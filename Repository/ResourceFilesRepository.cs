using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Repository
{
    /// <summary>
    /// Реализация сервиса для работы с источником данных в виде файлов ресурсов.
    /// </summary>
    public class ResourceFilesRepository : IRepository
    {
        private readonly ResourceManager _resourceManager;
        private readonly ILogger<ResourceFilesRepository> _logger;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="ResourceFilesRepository"/>.
        /// </summary>
        /// <param name="resourcePath">Путь к файлу ресурсов.</param>
        /// <param name="assembly">Сборка, содержащая файлы ресурсов.</param>
        /// <param name="logger">Логгер.</param>
        public ResourceFilesRepository(string resourcePath, Assembly assembly, ILogger<ResourceFilesRepository> logger)
        {
            _logger = logger;
            _resourceManager = new ResourceManager(resourcePath, assembly);
        }

        /// <summary>
        /// Получает локализованную строку по идентификатору и культуре.
        /// </summary>
        /// <param name="stringId">Идентификатор строки.</param>
        /// <param name="cultureInfo">Культура.</param>
        /// <returns>Найденная в файле ресурсов строка либо null, если строка не найдена.</returns>
        public string? GetLocalizedString(Guid stringId, CultureInfo cultureInfo)
        {
            try
            {
                return _resourceManager.GetString(stringId.ToString(), cultureInfo);
            }
            catch (MissingManifestResourceException ex)
            {
                _logger.LogWarning(ex, $"В файлах ресурсов не найдена строка {stringId} для культуры {cultureInfo.Name}.");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    $"Ошибка при поиске строки {stringId} для культуры {cultureInfo.Name} в источнике {typeof(ResourceFilesRepository)}.");
                throw;
            }
        }

        public bool Equals(object? x, object? y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;
            return Equals(x as ResourceFilesRepository, y as ResourceFilesRepository);
        }

        public bool Equals(ResourceFilesRepository? x, ResourceFilesRepository? y)
        {
            if (x == null || y == null)
                return false;
            return x._resourceManager.BaseName.Equals(y._resourceManager.BaseName);
        }

        public int GetHashCode(object obj)
        {
            return base.GetHashCode();
        }
    }
}
