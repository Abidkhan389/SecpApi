namespace Paradigm.Server
{
    using System;
    using System.Linq;
    using System.Text;
    using System.IO.Compression;
    using System.Threading.Tasks;
    using System.Text.RegularExpressions;    
    
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.WebUtilities;
    using Microsoft.AspNetCore.ResponseCompression;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authentication.JwtBearer;

    using Microsoft.Net.Http.Headers;

    using Microsoft.IdentityModel.Tokens;

    using Microsoft.Extensions.Options;    
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    using Paradigm.Service;
    using Paradigm.Server.Model;
    using Paradigm.Server.Formatter;
    using Paradigm.Server.Resolver;    

    public static partial class Extensions
    {
        private const string DefaultEnvironment = "production";

        public static IConfigurationBuilder AddConfiguration(this IConfigurationBuilder builder)
        {
            builder.AddEnvironmentVariables("ASPNETCORE_");

            var env = builder.Build().GetSection(WebHostDefaults.EnvironmentKey).Value;

            if (string.IsNullOrWhiteSpace(env))
            {
                env = DefaultEnvironment;
                Console.WriteLine($"WARN: Required runtime variable ASPNETCORE_ENVIRONMENT not found. Default set to '{env}'");
            }

            builder.AddJsonFile($"app.settings.json", optional: true);
            builder.AddJsonFile($"app.{env}.json", optional: false);

            return builder;
        }        

        public static void AddSystemConfiguration(this IServiceCollection services)
        {
            services.Configure<AppConfig>(WebApp.Configuration);
            services.Configure<Service.Config>(WebApp.Configuration.GetSection("service"));
            services.Configure<TokenProviderConfig>(WebApp.Configuration.GetSection("service:tokenProvider"));
            services.Configure<Data.Config>(WebApp.Configuration.GetSection("data"));
            services.Configure<Config>(WebApp.Configuration.GetSection("server"));
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);

            services.AddResponseCompression(options =>
            {
                options.MimeTypes = new[]
                {
                    // Default
                    "text/plain",
                    "text/css",
                    "application/javascript",
                    "text/html",
                    "application/xml",
                    "text/xml",
                    "application/json",
                    "text/json",
                    // Custom
                    "image/svg+xml"
                };
            });

            services.AddMemoryCache();
            services.AddDetection();
        }

        public static void UseConfigurationMiddleware(this IApplicationBuilder app)
        {
            app.UseDefaultFiles();

            app.UseResponseCompression();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                //endpoints.MapHealthChecks("/engine/health");
            });
        }

        public static void AddConfigureMvc(this IServiceCollection services, AntiForgeryConfig xsrfConfig)
        {
            services.AddControllers()
                    .AddNewtonsoftJson(options =>
                    {
                        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                        options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Include;
                        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    });
            services.AddRazorPages();

            services.AddControllersWithViews(options =>
                options.Filters.Add<Handlers.GlobalExceptionFilter>());

            services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<LocalizationOptions>, LocalizationResolverOptions>());

            services.AddAntiforgery(options =>
            {
                options.Cookie.Name = xsrfConfig.CookieName;
                options.HeaderName = xsrfConfig.HeaderName;
                options.Cookie.SecurePolicy = CookieSecurePolicy.None;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(Security.AuthorizeClaimAttribute.PolicyName, o => {
                    o.RequireAssertion(Security.AuthorizeClaimAttribute.PolicyHandler);
                });
            });
        }

        public static void AddConfigureAuthentication(this IServiceCollection services, TokenProviderConfig cfg, string[] areas)
        {
            SymmetricSecurityKey signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(cfg.TokenSecurityKey));

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = cfg.TokenIssuer,
                ValidateAudience = true,
                ValidAudience = cfg.TokenAudience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = (context) =>
                    {
                        return OnChallenge(context, areas);
                    }
                };
                options.SaveToken = true;
                options.TokenValidationParameters = tokenValidationParameters;
            })
            .AddCookie(options =>
            {
                options.Cookie.Name = "access_token";
                options.TicketDataFormat = new TokenDataFormat(cfg.TokenSecurityAlgorithm, CookieAuthenticationDefaults.AuthenticationScheme, tokenValidationParameters);
            });
        }

        private static Task OnChallenge(JwtBearerChallengeContext context, string[] areas)
        {
            if (context.AuthenticateFailure != null)
            {
                string location = CreateReturnLocation(context, areas);

                context.Response.Headers.Append(HeaderNames.Location, location);
                context.Response.Headers.Append(HeaderNames.WWWAuthenticate, context.Options.Challenge);

                if (context.Request.AcceptsJsonResponse())
                {
                    return Task.Factory.StartNew(() =>
                    {
                        context.Response.StatusCode = 401;
                        context.HandleResponse();
                    });
                }
            }

            return Task.Factory.StartNew(() => context.HandleResponse());
        }

        private static string CreateReturnLocation(JwtBearerChallengeContext context, string[] areas)
        {
            string locationHeader = context.Request.Headers[HeaderNames.Location];

            Uri referrer = new Uri(context.Request.Headers[HeaderNames.Referer]);
            Uri location = new Uri(locationHeader ?? referrer.ToString());

            string returnUrl = CreateReturnUrl(referrer, areas);

            string locationUri = QueryHelpers.AddQueryString("Login", "returnUrl", returnUrl);

            if (!string.IsNullOrEmpty(context.Error))
                locationUri = QueryHelpers.AddQueryString(locationUri, "errorCode", context.Error);

            if (!string.IsNullOrEmpty(context.AuthenticateFailure.Message))
                locationUri = QueryHelpers.AddQueryString(locationUri, "errorDesc", context.ErrorDescription);

            return locationUri;
        }

        private static string CreateReturnUrl(Uri referrer, string[] areas)
        {
            string areaPattern = string.Join(string.Empty, areas.Select(o => "/" + o));

            Regex regex = new Regex($"({areaPattern})");

            return regex.Replace(referrer.ToString(), string.Empty, 1);
        }

    }
}