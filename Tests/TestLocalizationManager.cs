using System.Globalization;
using System.Reflection;
using BusinessLogic;
using Microsoft.Extensions.Logging;
using Moq;
using Repository;

namespace Tests
{
    [TestFixture]
    public class TestLocalizationManager
    {
        private const string ResourceFileFolder = "ResourceFiles";
        private const string ResourceFileName = "Resource";
        private const string FilledXmlFileName = "TestXML.xml";
        private const string EmptyXmlFileName = "EmptyXML.xml";

        private ILogger<LocalizationManager> _LocalizationManagerLoggerMock;

        [SetUp]
        public void Setup()
        {
            _LocalizationManagerLoggerMock = new Mock<ILogger<LocalizationManager>>().Object;
        }

        //Так как тесты больше для демонстрации работоспособности системы - используются реализации IRepository, а не замоканные аналоги. 

        /// <summary>
        /// Один источник данных, ноль строк по идентификатору.
        /// </summary>
        [Test]
        public void OneSourceNullStrings()
        {
            var localizationManager = new LocalizationManager(_LocalizationManagerLoggerMock);
            localizationManager.RegisterSource(CreateCorrectXmlRepository(FilledXmlFileName));
            var ruString = localizationManager.GetString(Guid.Parse("6c2c210c-8fd6-449d-980b-97ad6f6362e1"), new CultureInfo("ru-RU"));
            Assert.That(ruString, Is.Null);
        }

        /// <summary>
        /// Один источник данных, одна строка по идентификатору, есть подходящая культура.
        /// </summary>
        [Test]
        public void OneSourceOneStringHasSuitableCulture()
        {
            var localizationManager = new LocalizationManager(_LocalizationManagerLoggerMock);
            localizationManager.RegisterSource(CreateCorrectXmlRepository(FilledXmlFileName));
            var esString = localizationManager.GetString(Guid.Parse("49e65dfd-5896-4428-bf44-c14fc250f214"), new CultureInfo("es-ES"));
            Assert.That(esString, Is.EqualTo("Identificador unico."));
        }

        /// <summary>
        /// Один источник данных, одна строка по идентификатору, нет подходящей культуры.
        /// </summary>
        [Test]
        public void OneSourceOneStringNoSuitableCulture()
        {
            var localizationManager = new LocalizationManager(_LocalizationManagerLoggerMock);
            localizationManager.RegisterSource(CreateCorrectXmlRepository(FilledXmlFileName));
            var frString = localizationManager.GetString(Guid.Parse("49e65dfd-5896-4428-bf44-c14fc250f214"), new CultureInfo("fr-FR"));
            Assert.That(frString, Is.Null);
        }

        /// <summary>
        /// Один источник данных, одна строка по идентификатору, много культур.
        /// </summary>
        [Test]
        public void OneSourceOneStringMultipleCultures()
        {
            var localizationManager = new LocalizationManager(_LocalizationManagerLoggerMock);
            localizationManager.RegisterSource(CreateCorrectXmlRepository(FilledXmlFileName));
            var ruString = localizationManager.GetString(Guid.Parse("6c2c210c-8fd6-449d-980b-97ad6f6362e7"), new CultureInfo("ru-RU"));
            Assert.That(ruString, Is.EqualTo("Сервис."));
            var enString = localizationManager.GetString(Guid.Parse("6c2c210c-8fd6-449d-980b-97ad6f6362e7"), new CultureInfo("en-US"));
            Assert.That(enString, Is.EqualTo("Service."));
        }

        /// <summary>
        /// Несколько источников данных, ноль строк по идентификатору.
        /// </summary>
        [Test]
        public void MultipleSourcesNullStrings()
        {
            var localizationManager = new LocalizationManager(_LocalizationManagerLoggerMock);
            localizationManager.RegisterSource(CreateCorrectXmlRepository(FilledXmlFileName));
            localizationManager.RegisterSource(CreateResourceFilesRepository());
            var ruString = localizationManager.GetString(Guid.Parse("6c2c210c-8fd6-449d-980b-97ad6f6362e1"), new CultureInfo("ru-RU"));
            Assert.That(ruString, Is.Null);
        }

        /// <summary>
        /// Несколько источников данных, одна строка по идентификатору в первом источнике.
        /// </summary>
        [Test]
        public void MultipleSourcesOneStringFirstSource()
        {
            var localizationManager = new LocalizationManager(_LocalizationManagerLoggerMock);
            localizationManager.RegisterSource(CreateCorrectXmlRepository(FilledXmlFileName));
            localizationManager.RegisterSource(CreateResourceFilesRepository());
            var esString = localizationManager.GetString(Guid.Parse("49e65dfd-5896-4428-bf44-c14fc250f214"), new CultureInfo("es-ES"));
            Assert.That(esString, Is.EqualTo("Identificador unico."));
        }

        /// <summary>
        /// Несколько источников данных, одна строка по идентификатору в последнем источнике.
        /// </summary>
        [Test]
        public void MultipleSourcesOneStringLastSource()
        {
            var localizationManager = new LocalizationManager(_LocalizationManagerLoggerMock);
            localizationManager.RegisterSource(CreateCorrectXmlRepository(FilledXmlFileName));
            localizationManager.RegisterSource(CreateResourceFilesRepository());
            var ruString = localizationManager.GetString(Guid.Parse("2868cf65-417d-4257-8bde-1d5ea318e98a"), new CultureInfo("ru-RU"));
            Assert.That(ruString, Is.EqualTo("Уникальный идентификатор."));
        }

        /// <summary>
        /// Несколько источников данных, в каждом источнике есть локализация по идентификатору,
        /// должна выбраться строка из первого по приоритету добавления источника.
        /// </summary>
        [Test]
        public void MultipleSourcesMultipleStrings()
        {
            var localizationManager = new LocalizationManager(_LocalizationManagerLoggerMock);
            localizationManager.RegisterSource(CreateCorrectXmlRepository(FilledXmlFileName));
            localizationManager.RegisterSource(CreateResourceFilesRepository());
            var ruString = localizationManager.GetString(Guid.Parse("a15aa672-3846-4e84-a644-b99feee55205"), new CultureInfo("ru-RU"));
            Assert.That(ruString, Is.EqualTo("Есть во всех источниках (xml файл)."));
        }

        /// <summary>
        /// Один источник данных, две локализации одной строки, одна из них с текущей культурой потока, вторая нет.
        /// Запрос строки без культуры.
        /// </summary>
        [Test]
        public void OneSourcesMultipleStringsCurrentThreadCulture()
        {
            var arCurrentCulture = new CultureInfo("ar-AE");
            Thread.CurrentThread.CurrentCulture = arCurrentCulture; //т.к. каждый тест в своём потоке.
            Assert.That(CultureInfo.CurrentCulture, Is.EqualTo(arCurrentCulture));
            var localizationManager = new LocalizationManager(_LocalizationManagerLoggerMock);
            localizationManager.RegisterSource(CreateCorrectXmlRepository(FilledXmlFileName));
            var ruString = localizationManager.GetString(Guid.Parse("54effa47-12c2-4363-bb4e-884a8f752108"), null);
            Assert.That(ruString, Is.EqualTo("Two localizations for one string (ar-AE)."));
        }

        /// <summary>
        /// Один источник данных. Должен вернуться true.
        /// </summary>
        [Test]
        public void OneSourceReturnsTrueOnAdd()
        {
            var localizationManager = new LocalizationManager(_LocalizationManagerLoggerMock);
            var xmlRepo = CreateCorrectXmlRepository(FilledXmlFileName);
            Assert.That(localizationManager.RegisterSource(xmlRepo), Is.True);
        }

        /// <summary>
        /// Два одинаковых источника данных. Должен вернуться false при попытке добавления второго источника.
        /// </summary>
        [Test]
        public void TwoEqualSourcesReturnFalseOnAdd()
        {
            var localizationManager = new LocalizationManager(_LocalizationManagerLoggerMock);
            var xmlRepo = CreateCorrectXmlRepository(FilledXmlFileName);
            Assert.That(localizationManager.RegisterSource(xmlRepo), Is.True);
            Assert.That(localizationManager.RegisterSource(xmlRepo), Is.False);
        }

        /// <summary>
        /// Два источника данных одного типа, но не равные. Должен вернуться true при попытке добавления второго источника.
        /// </summary>
        [Test]
        public void TwoNonEqualSameTypeSourcesReturnTrueOnAdd()
        {
            var localizationManager = new LocalizationManager(_LocalizationManagerLoggerMock);
            var xmlRepo1 = CreateCorrectXmlRepository(FilledXmlFileName);
            var xmlRepo2 = CreateCorrectXmlRepository(EmptyXmlFileName);
            Assert.That(localizationManager.RegisterSource(xmlRepo1), Is.True);
            Assert.That(localizationManager.RegisterSource(xmlRepo2), Is.True);
        }

        /// <summary>
        /// Проверка выброса исключения.
        /// </summary>
        [Test]
        public void OneSourceThrowsException()
        {
            var localizationManager = new LocalizationManager(_LocalizationManagerLoggerMock);
            var guid = Guid.NewGuid();
            var cultureInfo = new CultureInfo("ru-RU");
            var mockIRepository = new Mock<IRepository>();
            mockIRepository
                .Setup(x => x.GetLocalizedString(guid, cultureInfo))
                .Throws(new Exception("Неизвестная ошибка."));
            localizationManager.RegisterSource(mockIRepository.Object);
            Assert.Throws<LocalizationException>(() => localizationManager.GetString(guid, cultureInfo));
        }

        /// <summary>
        /// Проверка работы LocalizationManager-а со всем типами источников данных.
        /// </summary>
        [Test, TestCaseSource(nameof(OneSourceOneStringTestCases))]
        public void GetStringFromEverySource(IRepository source, Guid id, CultureInfo cultureInfo, string expected)
        {
            var localizationManager = new LocalizationManager(_LocalizationManagerLoggerMock);
            Assert.That(localizationManager.RegisterSource(source), Is.True);
            var trString = localizationManager.GetString(id, cultureInfo);
            Assert.That(trString, Is.EqualTo(expected));
        }

        public static object[] OneSourceOneStringTestCases =
        {
            new object[]
            {
                CreateResourceFilesRepository(),
                Guid.Parse("6c2c210c-8fd6-449d-980b-97ad6f6362e7"),
                new CultureInfo("tr-TR"),
                "Merhaba"
            },
            new object[]
            {
                CreateCorrectXmlRepository(FilledXmlFileName),
                Guid.Parse("6c2c210c-8fd6-449d-980b-97ad6f6362e7"),
                new CultureInfo("ru-RU"),
                "Сервис."
            },
        };

        private static ResourceFilesRepository CreateResourceFilesRepository()
        {
            return new ResourceFilesRepository(
                $"{Assembly.GetExecutingAssembly().GetName().Name}.{ResourceFileFolder}.{ResourceFileName}",
                Assembly.GetExecutingAssembly(),
                new Mock<ILogger<ResourceFilesRepository>>().Object);
        }

        private static XmlRepository CreateCorrectXmlRepository(string xmlFileName)
        {
            var xsdFileName = "TestXMLSchema.xsd";
            return new XmlRepository(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ResourceFileFolder, xmlFileName),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ResourceFileFolder, xsdFileName),
                new Mock<ILogger<XmlRepository>>().Object);
        }
    }
}