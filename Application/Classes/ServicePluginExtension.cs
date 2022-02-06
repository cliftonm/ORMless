using System;
using System.Linq;
using System.Reflection;

using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Interfaces;
using Lib;

namespace Demo.Classes
{
    public static class ServicePluginExtension
    {
        public static IServiceCollection LoadPlugins(this IServiceCollection services, IConfiguration cfg)
        {
            // AppDomain.CurrentDomain.AssemblyResolve+=new ResolveEventHandler(MyResolveEventHandler);  
            AppSettings.Settings.Plugins.ForEach(p =>
            {
                Assembly assembly = Assembly.LoadFrom(p.Path);
                var part = new AssemblyPart(assembly);
                // services.AddControllers().PartManager.ApplicationParts.Add(part);
                // Correction from Colin O'Keefe so that things like customizing the routing or API versioning works,
                // which gets ignored using the above commented out AddControllers line.
                services.AddControllersWithViews().ConfigureApplicationPartManager(apm => apm.ApplicationParts.Add(part));

                var atypes = assembly.GetTypes();
                var pluginClass = atypes.SingleOrDefault(t => t.GetInterface(nameof(IPlugin)) != null);

                if (pluginClass != null)
                {
                    var initMethod = pluginClass.GetMethod(nameof(IPlugin.Initialize), BindingFlags.Public | BindingFlags.Instance);
                    var obj = Activator.CreateInstance(pluginClass);
                    initMethod.Invoke(obj, new object[] { services, cfg });
                }
            });

            return services;
        }

        /*
        private static Assembly MyResolveEventHandler(object sender, ResolveEventArgs args)
        {
            return null;
        }
        */
    }
}
