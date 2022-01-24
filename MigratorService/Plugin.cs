using System.IO;
using System.Reflection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using FluentMigrator.Runner;

using Clifton;

using Interfaces;
using Lib;

namespace Clifton.Services
{
    public class Plugin : IPlugin
    {
        public void Initialize(IServiceCollection services, IConfiguration cfg)
        {
            services.AddScoped<IMigratorService, MigratorService>();

            string migrationAssemblyPath = Path.Combine(AppSettings.Settings.ExecutingAssembly.Location.LeftOfRightmostOf("\\"), "Migrations.dll");
            Assembly migrationAssembly = Assembly.LoadFrom(migrationAssemblyPath);
            var connection = cfg.GetConnectionString(AppSettings.Settings.UseDatabase);

            services.AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddSqlServer()
                    .WithGlobalConnectionString(connection)
                    .ScanIn(migrationAssembly).For.Migrations())
                    .AddLogging(lb => lb.AddFluentMigratorConsole());

        }
    }
}
