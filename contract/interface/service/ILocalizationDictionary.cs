namespace Paradigm.Contract.Interface
{
    using System;
    using System.Globalization;
    using System.Collections.Generic;

    using Paradigm.Contract.Model;

    public interface ILocalizationDictionary
    {
        ILocalizedString this[string key] { get; }
        CultureInfo Culture { get; }
        TimeZoneInfo TimeZone { get; }
        IEnumerable<string> Keys { get; }
        IEnumerable<ILocalizedString> Values { get; }
        bool ContainsKey(string key);
        bool TryGetValue(string key, out ILocalizedString value);
    }
}