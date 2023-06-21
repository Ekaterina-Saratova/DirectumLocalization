using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;

namespace Repository
{
    public class XmlRepository : IRepository
    {
        private readonly string _xmlFilePath;
        private readonly ILogger<XmlRepository> _logger;

        public XmlRepository(string xmlFilePath, string xsdFilePath, ILogger<XmlRepository> logger)
        {
            _logger = logger;
            try
            {
                CommonHelper.ValidateFilePath(xmlFilePath);
                CommonHelper.ValidateFilePath(xsdFilePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при инициализации класса {typeof(XmlRepository)}");
                throw;
            }
            ValidateXmlWithXsd(xmlFilePath, xsdFilePath);
            _xmlFilePath = xmlFilePath;
        }

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
            using (var reader = XmlReader.Create(_xmlFilePath))
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

        private void ValidateXmlWithXsd(string xmlFilePath, string xsdFilePath)
        {
            try
            {
                var xmlDocument = new XmlDocument();
                xmlDocument.Load(xmlFilePath);
                xmlDocument.Schemas.Add(null, xsdFilePath);
                xmlDocument.Validate(null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при валидации xml-файла {xmlFilePath} по схеме {xsdFilePath}.");
                throw;
            }
        }
    }
}
