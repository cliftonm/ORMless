using System.Reflection;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Lib
{
    public class AppSettings
    {
        [JsonIgnore]
        public static AppSettings Settings { get; set; }

        [JsonIgnore]
        public List<Plugin> Plugins { get; set; } = new List<Plugin>();

        [JsonIgnore]
        public Assembly ExecutingAssembly => Assembly.GetExecutingAssembly();

        public string UseDatabase { get; set; }

        public AppSettings()
        {
            Settings = this;
        }
    }
}