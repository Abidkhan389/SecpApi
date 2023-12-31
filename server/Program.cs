namespace Paradigm.Server
{
    using System.IO;
    using Microsoft.AspNetCore.Hosting;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Configuration;

    public class WebApp
    {
        internal static IConfigurationRoot Configuration;

        public static void Main(string[] args)
        {
            var root = Directory.GetCurrentDirectory();           

            Configuration = new ConfigurationBuilder()
                .SetBasePath(root)
                .AddConfiguration()
                .Build();

            IConfigurationSection logging = Configuration.GetSection("Logging");

            var host = new WebHostBuilder()
                .UseConfiguration(Configuration)
                .ConfigureLogging(factory =>
                {
                    if (logging.GetSection("Debug").Exists())
                        factory.AddConsole();
                    if (logging.GetSection("Console").Exists())
                        factory.AddDebug();
                })
                .UseKestrel()
                .UseContentRoot(root)
                .UseStartup<Startup>()
                .UseUrls("https://localhost:5000")
                //.UseUrls("http://115.0.9.161:5000")
                .Build();
                host.Run();
        }
    }
}
