using Interfaces;

namespace Plugin2
{
    public class Plugin2Service : IPlugin2Service
    {
        public int Add(int a, int b)
        {
            return a + b;
        }
    }
}
