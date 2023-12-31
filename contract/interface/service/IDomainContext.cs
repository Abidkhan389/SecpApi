namespace Paradigm.Contract.Interface
{
    using System;
    using System.Globalization;
    using Paradigm.Contract.Model;

    public interface IDomainContext
    {
        CultureInfo Culture { get; set; }
        TimeZoneInfo SourceTimeZone { get; set; }
        IUser User { get; set; }
    }
}