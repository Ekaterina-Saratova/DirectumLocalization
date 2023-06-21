using System.Collections;
using System.Globalization;

namespace Repository
{
    /// <summary>
    /// Сервис для работы с различными источниками данных.
    /// </summary>
    public interface IRepository : IEqualityComparer
    {
        /// <summary>
        /// Получает локализованную строку по идентификатору и культуре.
        /// </summary>
        /// <param name="stringId">Идентификатор строки.</param>
        /// <param name="cultureInfo">Культура.</param>
        /// <returns>Найденная в источнике данных строка либо null, если строка не найдена.</returns>
        string? GetLocalizedString(Guid stringId, CultureInfo cultureInfo);
    }
}