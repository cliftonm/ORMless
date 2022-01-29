using Microsoft.AspNetCore.Mvc;

using Lib;

namespace AuditService
{
    [ApiController]
    [Route("[controller]")]
    public class AuditController : PluginController
    {
        public override string Version => "1.00";
    }
}
