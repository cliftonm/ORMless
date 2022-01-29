using Microsoft.AspNetCore.Mvc;

using Lib;

namespace DatabaseService
{
    [ApiController]
    [Route("[controller]")]
    public class DatabaseController : PluginController
    {
        public override string Version => "1.00";
    }
}
