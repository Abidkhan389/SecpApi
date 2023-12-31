namespace Paradigm.Server
{
    using System;
    using System.Linq;
    using System.Globalization;
    using System.Collections.Generic;    

    using Microsoft.AspNetCore.Http;
    using Microsoft.Net.Http.Headers;
    using Microsoft.Extensions.Options;
    using Microsoft.AspNetCore.Localization;
    
    public class CultureService
    {
        public static string CultureCookieName = CookieRequestCultureProvider.DefaultCookieName;
        private readonly Config config;
        public const ushort CultureName = 0;
        public const ushort UICultureName = 1;
        public const ushort TimeZoneId = 2;

        public CultureService(IOptions<Config> config)
        {
            this.config = config.Value;
        }

        public void EnsureCookie(HttpContext httpContext)
        {
            if (!httpContext.Request.Cookies.ContainsKey(CultureCookieName))
            {
                string cultureName = this.config.DefaultCulture;
                string timeZoneId = this.config.DefaultTimeZone;

                if (string.IsNullOrWhiteSpace(cultureName))
                    cultureName = CultureInfo.CurrentUICulture.Name;

                if (string.IsNullOrWhiteSpace(timeZoneId))
                    timeZoneId = Data.Globalization.DefaultTimeZoneId;

                UpdateCookie(httpContext, cultureName, timeZoneId);
            }
        }

        public void RefreshCookie(HttpContext httpContext, string cultureName, string timeZoneId)
        {
            UpdateCookie(httpContext, cultureName, timeZoneId);
        }

        public string GetFromRequest(HttpContext httpContext, ushort value)
        {
            string cookieValue;

            if (httpContext.Request.Cookies.TryGetValue(CultureCookieName, out cookieValue))
            {
                string[] values = Parse(cookieValue);

                if (value < values.Length)
                    return values[value];
            }

            return null;
        }

        private static void UpdateCookie(HttpContext httpContext, string cultureName, string timeZoneId)
        {
            HttpResponse response = httpContext.Response;

            if (!response.HasStarted)
            {
                IList<SetCookieHeaderValue> cookies = response.GetTypedHeaders().SetCookie;

                if (cookies != null)
                {
                    var cookie = cookies.FirstOrDefault(o => o.Name == CultureCookieName);

                    if (cookie != null)
                        cookies.Remove(cookie);
                }

                string value = $"c={cultureName.Substring(0, 2)}|uic={cultureName}|t={timeZoneId}";

                response.Cookies.Append(CultureCookieName, value, new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });
            }
        }

        private static string[] Parse(string cookieValue)
        {
            if (cookieValue != null)
                return cookieValue.Split('|').Select(o => o.Split('=')[1]).ToArray();

            return null;
        }
    }
}
