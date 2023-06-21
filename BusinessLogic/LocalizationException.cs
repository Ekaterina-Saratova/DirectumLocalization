using System.Globalization;

namespace BusinessLogic
{
    public class LocalizationException : Exception
    {
        public LocalizationException()
        {
        }

        public LocalizationException(string? message) : base(message)
        {
        }

        public LocalizationException(
            string message,
            Exception innerException)
            : base(message, innerException)
        {
        }

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
