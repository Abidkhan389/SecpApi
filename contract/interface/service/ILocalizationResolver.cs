namespace Paradigm.Contract.Interface
{
    using System.Collections.Generic;

    using Paradigm.Contract.Model;

    public interface ILocalizationResolver
    {
        IEnumerable<IKeyValue> ResolveSupportedCultures();
        object ResolveCulture(string cultureName);
    }
}
