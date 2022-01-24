using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Interfaces;

namespace Plugin
{
    public class Plugin : IPlugin
    {
        public void Initialize(IServiceCollection services, IConfiguration cfg)
        {
            services.AddSingleton<IPluginService, PluginService>();
        }
    }
}
