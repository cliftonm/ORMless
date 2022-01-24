using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Interfaces;

namespace Plugin2
{
    public class Plugin2 : IPlugin
    {
        public void Initialize(IServiceCollection services, IConfiguration cfg)
        {
            services.AddSingleton<IPlugin2Service, Plugin2Service>();
        }
    }
}
