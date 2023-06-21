using Repository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    /// <summary>
    /// Интерфейс менеджера локализации.
    /// </summary>
    public interface ILocalizationManager
    {
        /// <summary>
        /// Получение строки локализации из зарегистрированных источников.
        /// </summary>
        /// <param name="stringId">Идентификатор строки локализации.</param>
        /// <param name="cultureInfo">Культура строки локализации.</param>
        /// <returns>Найденная строка либо null, если строки нет ни в одном из источников.</returns>
        string? GetString(Guid stringId, CultureInfo? cultureInfo);

        /// <summary>
        /// Регистрация источника строк локализации.
        /// </summary>
        /// <param name="repository">Источник строк локализации.</param>
        /// <returns>true, если регистрация успешна, false если нет.</returns>
        bool RegisterSource(IRepository repository);
    }
}
