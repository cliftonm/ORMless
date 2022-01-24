using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Interfaces;

namespace Clifton.Services
{
    public class Plugin : IPlugin
    {
        public void Initialize(IServiceCollection services, IConfiguration cfg)
        {
            services.AddScoped<ITableService, TableService>();
        }
    }
}
