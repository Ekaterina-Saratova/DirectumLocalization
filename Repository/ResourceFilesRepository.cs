using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Repository
{
    public class ResourceFilesRepository : IRepository
    {
        private readonly ResourceManager _resourceManager;
        private readonly ILogger<ResourceFilesRepository> _logger;

        public ResourceFilesRepository(string resourcePath, Assembly assembly, ILogger<ResourceFilesRepository> logger)
        {
            _logger = logger;
            _resourceManager = new ResourceManager(resourcePath, assembly);
        }

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
