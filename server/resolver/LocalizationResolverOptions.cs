namespace Paradigm.Server.Resolver
{
    using System.IO;    
    using Microsoft.Extensions.Options;

    using Paradigm.Server.Model;

    public class LocalizationResolverOptions : IConfigureOptions<LocalizationOptions>
    {
        private readonly Config config;

        public LocalizationResolverOptions(IOptions<Config> config)
        {
            this.config = config.Value;
        }

        public void Configure(LocalizationOptions options)
        {
            options.Directory = new DirectoryInfo("./i18n");
            options.Pattern = "{0}.json";
        }
    }
}
