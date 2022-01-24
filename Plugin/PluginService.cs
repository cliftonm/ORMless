using Interfaces;

namespace Plugin
{
    public class PluginService : IPluginService
    {
        public string Test()
        {
            return "Tested!";
        }
    }
}
