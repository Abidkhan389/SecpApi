namespace Paradigm.Data
{
    using System;

    public static class Globalization
    {
        public static string DefaultTimeZoneId
        {
            get
            {
                return TimeZoneInfo.Local.Id;
            }
        }
    }
}