using System.Reflection;
using System.Collections.Generic;

namespace Lib
{
    public class AppSettings
    {
        public static AppSettings Settings { get; set; }
        public List<Plugin> Plugins { get; set; } = new List<Plugin>();
        public Assembly ExecutingAssembly => Assembly.GetExecutingAssembly();

        public string UseDatabase { get; set; }

        public AppSettings()
        {
            Settings = this;
        }
    }
}