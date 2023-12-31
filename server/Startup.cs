namespace Paradigm.Server
{
    using System;
    using System.IO;

    using Microsoft.OpenApi.Models;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using StructureMap;

    using Paradigm.Data;
    using Paradigm.Common;
    using Paradigm.Contract.Interface;
    using Paradigm.Data.ViewModels;

    public partial class Startup
    {
        AppConfig config = WebApp.Configuration.Get<AppConfig>();

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Paradigm"));
            }
            app.UseConfigurationMiddleware();

            app.UseAntiforgeryMiddleware(config.Server.AntiForgery.ClientName);
            app.UseRequestLocalization();
            app.UseHistoryModeMiddleware(new DirectoryInfo(config.Server.Webroot).FullName, config.Server.Areas);

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var dbContext = serviceScope.ServiceProvider.GetService<DbContextBase>())
                {
                    ICryptoService crypto = app.ApplicationServices.GetRequiredService<ICryptoService>();
                    dbContext.Database.Migrate();
                    dbContext.EnsureSeedData(crypto);
                }
            }
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddSystemConfiguration();
            //services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            services.AddConfigureAuthentication(config.Service.TokenProvider, new string[] { "admin" });
            var securityReq = new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            };

            services.AddConfigureMvc(config.Server.AntiForgery);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Paradigm Api",
                    Version = "v1",
                    Contact = new OpenApiContact()
                    {
                        Name = "Mubashar Iqbal",
                        Email = "mubashar.iqbal@khazanapk.com"
                    }
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JSON Web Token based security",
                });
                c.AddSecurityRequirement(securityReq);
            });

            services.AddDbContext<DatabaseContext>(options =>
            {
                string assemblyName = typeof(Data.Config).GetAssemblyName();
                options
                    .UseLazyLoadingProxies()
                    .UseNpgsql(config.Data.ConnectionString, s => s.MigrationsAssembly(assemblyName));
            });
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            var container = new Container(c =>
            {
                var registry = new Registry();

                registry.IncludeRegistry<ContainerRegistry>();
                registry.IncludeRegistry<Data.ContainerRegistry>();
                registry.IncludeRegistry<Common.ContainerRegistry>();
                registry.IncludeRegistry<Service.ContainerRegistry>();

                c.AddRegistry(registry);
                c.Populate(services);
            });

            return container.GetInstance<IServiceProvider>();
        }
    }
}
