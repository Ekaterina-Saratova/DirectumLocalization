using System.Collections;
using System.Globalization;

namespace Repository
{
    public interface IRepository : IEqualityComparer
    {
        string? GetLocalizedString(Guid stringId, CultureInfo cultureInfo);
    }
}