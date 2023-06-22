using System.Globalization;
using Repository;
using Microsoft.Extensions.Logging;

namespace BusinessLogic
{
    /// <summary>
    /// Менеджер локализации.
    /// </summary>
    public class LocalizationManager : ILocalizationManager
    {
        /// <summary>
        /// Список зарегистрированных источников.
        /// </summary>
        private readonly List<IRepository> _sourceList = new();
        private readonly ILogger<LocalizationManager> _logger;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="LocalizationManager"/>.
        /// </summary>
        /// <param name="logger">Логгер.</param>
        public LocalizationManager(ILogger<LocalizationManager> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Получение строки локализации из зарегистрированных источников.
        /// Приоритет по очереди добавления в список: самый первый - самый приоритетный.
        /// </summary>
        /// <param name="stringId">Идентификатор строки локализации.</param>
        /// <param name="cultureInfo">Культура строки локализации.</param>
        /// <returns>Найденная в одном из источников (согласно приоритету) строка либо null, если строки нет ни в одном из источников.</returns>
        public string? GetString(Guid stringId, CultureInfo? cultureInfo = null)
        {
            //Рассматривала вариант обертки возвращаемого значения в класс с хранением id, культуры и значения,
            //но в поставноке задачи метод должен "возвращать значение", поэтому - строка.
            try
            {
                foreach (var source in _sourceList)
                {
                    var result = source.GetLocalizedString(stringId, cultureInfo ?? CultureInfo.CurrentCulture);
                    if (result != null)
                        return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                var customEx = new LocalizationException(stringId, cultureInfo, ex);
                _logger.LogError(ex, ex.Message);
                throw customEx;
            }
        }

        /// <summary>
        /// Регистрация источника строк локализации.
        /// </summary>
        /// <param name="repository">Источник строк локализации.</param>
        /// <returns>true, если регистрация успешна, false если нет.</returns>
        public bool RegisterSource(IRepository repository)
        {
            if (_sourceList.Contains(repository)) //Сложность О(n).
                return false;
            _sourceList.Add(repository);
            return true;
        }
    }
}