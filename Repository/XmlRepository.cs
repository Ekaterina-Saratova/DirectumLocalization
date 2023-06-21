using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;

namespace Repository
{
    /// <summary>
    /// Реализация сервиса для работы с источником данных в виде xml файла.
    /// </summary>
    public class XmlRepository : IRepository
    {
        private readonly string _xmlFilePath;
        private readonly ILogger<XmlRepository> _logger;
        private readonly XmlReaderSettings _xmlReaderSettings;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="XmlRepository"/>.
        /// </summary>
        /// <param name="xmlFilePath">Путь к xml-файлу с данными.</param>
        /// <param name="xsdFilePath">Путь к xsd файлу.</param>
        /// <param name="logger">Логгер.</param>
        public XmlRepository(string xmlFilePath, string xsdFilePath, ILogger<XmlRepository> logger)
        {
            _logger = logger;
            try
            {
                File.Exists(xmlFilePath);
                File.Exists(xsdFilePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при инициализации класса {typeof(XmlRepository)}");
                throw;
            }

            _xmlFilePath = xmlFilePath;
            _xmlReaderSettings = new XmlReaderSettings();
            _xmlReaderSettings.Schemas.Add(null, xsdFilePath);
            _xmlReaderSettings.ValidationType = ValidationType.Schema;
        }

        /// <summary>
        /// Получает локализованную строку по идентификатору и культуре.
        /// </summary>
        /// <param name="stringId">Идентификатор строки.</param>
        /// <param name="cultureInfo">Культура.</param>
        /// <returns>Найденная в xml файле строка либо null, если строка не найдена.</returns>
        public string? GetLocalizedString(Guid stringId, CultureInfo cultureInfo)
        {
            try
            {
                return GetXmlElementsByNameAndAttrValue("locale", "name", cultureInfo.Name)
                    .Descendants("text")
                    .FirstOrDefault(e => GetAttributeValueByName(e, "id") == stringId.ToString())
                    ?.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    $"Ошибка при попытке получения строки {stringId} для культуры {cultureInfo.Name}");
                throw;
            }
        }

        private IEnumerable<XElement> GetXmlElementsByNameAndAttrValue(string elementName, string attrName, string attrValue)
        {
            using (var reader = XmlReader.Create(_xmlFilePath, _xmlReaderSettings))
            {
                while (reader.Read())
                {
                    if (reader.NodeType != XmlNodeType.Element || reader.Name != elementName 
                        || reader.GetAttribute(attrName) != attrValue) continue;
                    if (XNode.ReadFrom(reader) is XElement element)
                        yield return element;
                }
            }
        }

        private string? GetAttributeValueByName(XElement element, string attrName)
        {
            return element.Attribute(attrName)?.Value;
        }

        public bool Equals(object? x, object? y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;
            return Equals(x as XmlRepository, y as XmlRepository);
        }

        public bool Equals(XmlRepository? x, XmlRepository? y)
        {
            if (x == null || y == null)
                return false;
            return x._xmlFilePath.Equals(y._xmlFilePath);
        }

        public int GetHashCode(object obj)
        {
            return base.GetHashCode();
        }
    }
}
