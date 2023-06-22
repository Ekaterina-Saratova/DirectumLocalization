using System.Globalization;

namespace BusinessLogic
{
    /// <summary>
    /// Исключение, которое транслируется при ошибках в <see cref="ILocalizationManager"/>.
    /// </summary>
    public class LocalizationException : Exception
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="LocalizationException"/>.
        /// </summary>
        public LocalizationException()
        {
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="LocalizationException"/>.
        /// </summary>
        /// <param name="message">Сообщение об ошибке.</param>
        public LocalizationException(string? message) : base(message)
        {
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="LocalizationException"/>.
        /// </summary>
        /// <param name="message">Сообщение об ошибке.</param>
        /// <param name="innerException">Исключение, которое является причиной текущего исключения.</param>
        public LocalizationException(
            string message,
            Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="LocalizationException"/>.
        /// </summary>
        /// <param name="id">Идентификатор строки локализации.</param>
        /// <param name="cultureInfo">Культура строки локализации.</param>
        /// <param name="innerException">Исключение, которое является причиной текущего исключения.</param>
        public LocalizationException(
            Guid id,
            CultureInfo? cultureInfo,
            Exception innerException)
            : base(MakeMessage(id, cultureInfo), innerException)
        {
        }

        private static string MakeMessage(Guid id, CultureInfo? cultureInfo)
        {
            var cultureMessage = cultureInfo == null ? "по умолчанию" : cultureInfo.Name;
            return $"Ошибка при попытке получения строки {id} для культуры {cultureMessage}.";
        }
    }
}
